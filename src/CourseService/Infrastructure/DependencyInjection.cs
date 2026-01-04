using CoursePlatform.ServiceDefaults.Auth;
using Courses.Application.Abstractions;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.Repositories;
using Courses.Infrastructure.Database;
using Courses.Infrastructure.DomainEvents;
using Courses.Infrastructure.Repositories;
using Kernel.Auth.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Courses.Infrastructure;

public static class DependencyInjection
{
    internal const string ReadDatabaseConnectionSection = "ReadDatabase";
    internal const string WriteDatabaseConnectionSection = "WriteDatabase";
    internal const string RabbitMqConnectionSection = "RabbitMq";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        string authUrl = configuration["services:authservice:https:0"]
              ?? configuration["services:authservice:http:0"] ?? string.Empty;

        return services
            .AddServices()
            .AddDatabases(configuration)
            .AddMassTransitInternal(configuration)
            .AddHealthChecksInternal(configuration)
            .AddAuthenticationInternal(configuration)
            .AddInternalAuth(authUrl);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFeaturedCoursesRepository, FeaturedCoursesRepo>();
        services.AddScoped<IUrlResolver, UrlResolver.UrlResolver>();

        return services;
    }

    private static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddReadDatabase(configuration)
            .AddWriteDatabase(configuration);
    }

    private static IServiceCollection AddReadDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReadDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString(ReadDatabaseConnectionSection)!;

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<ReadDbContext>());

        return services;
    }

    private static IServiceCollection AddWriteDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString(WriteDatabaseConnectionSection)!;

            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName);
                })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IWriteDbContext>(sp => sp.GetRequiredService<WriteDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecksInternal(this IServiceCollection services, IConfiguration configuration)
    {
        //services
        //    .AddHealthChecks()
        //    .AddNpgSql(configuration.GetConnectionString("Database")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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
                var connectionString = configuration.GetConnectionString(RabbitMqConnectionSection)!;

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
