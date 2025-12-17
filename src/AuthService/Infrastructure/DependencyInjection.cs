using Application.Abstractions.Data;
using Application.Abstractions.MessageQueue;
using CoursePlatform.ServiceDefaults.Auth;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.Extensions;
using Infrastructure.MassTransit;
using Infrastructure.Redis.Extensions;
using Kernel.Auth.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    internal const string WriteDbSectionName = "WriteDatabase";
    internal const string ReadDbSectionName = "ReadDatabase";
    internal const string RabbitMqSectionName = "RabbitMq";
    internal const string RedisConnectionName = "redis";

    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddInfrastructureDefaults();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.AddDistributedPermissionStore(RedisConnectionName);

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseInfrastructureDefaultEndpoints();

        return app;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddServices()
            .AddDatabase(configuration)
            .AddMassTransitInternal(configuration)
            .AddHealthChecksInternal(configuration)
            .AddUserContextProvider();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }

    private static IServiceCollection AddUserContextProvider(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(WriteDbSectionName)
                ?? throw new InvalidOperationException("Database connection string not found");

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddDbContext<ReadDbContext>((serviceProvider, options) =>
        {
            string connectionString = configuration.GetConnectionString(ReadDbSectionName)
                ?? throw new InvalidOperationException("Database connection string not found");

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IWriteDbContext>(sp => sp.GetRequiredService<WriteDbContext>());
        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<ReadDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WriteDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecksInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }

    private static IServiceCollection AddMassTransitInternal(
        this IServiceCollection services,
        IConfiguration configuration)
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
                string connectionString = configuration.GetConnectionString(RabbitMqSectionName)
                    ?? throw new InvalidOperationException("RabbitMQ connection string not found");

                busConfig.Host(new Uri(connectionString), h => { });
                busConfig.ConfigureEndpoints(context);
            });

            config.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("ready");
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }
}