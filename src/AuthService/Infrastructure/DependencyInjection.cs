using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.MassTransit;
using Infrastructure.Security;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    private static readonly string _databaseSectionName = "Database";
    private static readonly string _rabbitMqSectionName = "RabbitMq";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddMassTransitInternal(configuration)
            .AddHealthChecksInternal(configuration);

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(_databaseSectionName)
                ?? throw new InvalidOperationException("Database connection string not found");

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IWriteDbContext>(sp => sp.GetRequiredService<AuthDbContext>());
        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<AuthDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecksInternal(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Health checks can be added here
        return services;
    }

    private static IServiceCollection AddMassTransitInternal(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(typeof(DependencyInjection).Assembly);

            config.AddEntityFrameworkOutbox<AuthDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
                o.QueryDelay = TimeSpan.FromSeconds(30);
            });

            config.AddConfigureEndpointsCallback((ctx, endpointName, endpointCfg) =>
            {
                endpointCfg.UseEntityFrameworkOutbox<AuthDbContext>(ctx);
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
                string connectionString = configuration.GetConnectionString(_rabbitMqSectionName)
                    ?? throw new InvalidOperationException("RabbitMQ connection string not found");

                busConfig.Host(new Uri(connectionString), h => { });
                busConfig.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }
}
