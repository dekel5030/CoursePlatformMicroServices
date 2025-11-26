using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Events;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.Jwt;
using Infrastructure.MassTransit;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    private static readonly string _readDatabaseConnectionStringName = "ReadDatabase";
    private static readonly string _writeDatabaseConnectionStringName = "WriteDatabase";
    private static readonly string _rabbitMqSectionName = "RabbitMq";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddMassTransitInternal(configuration);
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        
        // Register domain event handlers in Infrastructure layer
        services.AddScoped<IDomainEventHandler<UserProfileCreatedDomainEvent>, UserProfileCreatedDomainEventHandler>();

        services.ConfigureJwtAuthentication(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReadDatabases(configuration);
        services.AddWriteDatabase(configuration);

        return services;
    }

    private static IServiceCollection AddReadDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReadDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(_readDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IReadDbContext>(serviceProvider => serviceProvider.GetRequiredService<ReadDbContext>());

        return services;
    }

    private static IServiceCollection AddWriteDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(_writeDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IWriteDbContext>(serviceProvider => serviceProvider.GetRequiredService<WriteDbContext>());

        return services;
    }

    private static IServiceCollection AddMassTransitInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(typeof(DependencyInjection).Assembly);

            config.AddEntityFrameworkOutbox<WriteDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
                o.QueryDelay = TimeSpan.FromSeconds(30);
            });


            config.AddConfigureEndpointsCallback((ctx, endpointName, endpointCfg) =>
            {
                endpointCfg.UseEntityFrameworkOutbox<WriteDbContext>(ctx);
                endpointCfg.UseMessageRetry(r =>
                {
                    r.Handle<InvalidOperationException>();
                    r.Intervals(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(40));
                });
            });

            config.UsingRabbitMq((context, busConfig) =>
            {
                string connectionString = configuration.GetConnectionString(_rabbitMqSectionName)!;

                busConfig.Host(new Uri(connectionString!), h => { });
                busConfig.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }
}
