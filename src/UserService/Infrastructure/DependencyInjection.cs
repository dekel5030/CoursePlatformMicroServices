using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Users.Application.Abstractions.Context;
using Users.Application.Abstractions.Data;
using Users.Infrastructure.Auth;
using Users.Infrastructure.Database;
using Users.Infrastructure.DomainEvents;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    internal const string ReadDatabaseConnectionStringName = "ReadDatabase";
    internal const string WriteDatabaseConnectionStringName = "WriteDatabase";
    internal const string RabbitMqConnectionStringName = "RabbitMq";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddMassTransitInternal(configuration);
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        //services.ConfigureJwtAuthentication(configuration);
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        //app.UseAuthentication();
        //app.UseAuthorization();

        return app;
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
            var connectionString = configuration.GetConnectionString(ReadDatabaseConnectionStringName)!;

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IReadDbContext>(serviceProvider => serviceProvider.GetRequiredService<ReadDbContext>());

        return services;
    }

    private static IServiceCollection AddWriteDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString(WriteDatabaseConnectionStringName)!;

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
                var connectionString = configuration.GetConnectionString(RabbitMqConnectionStringName)!;

                busConfig.Host(new Uri(connectionString!), h => { });
                busConfig.ConfigureEndpoints(context);
            });

            config.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("ready");
            });
        });


        return services;
    }
}