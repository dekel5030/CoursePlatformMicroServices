using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();

        var healthChecks = builder.Services.AddHealthChecks();

        var readDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.ReadDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(readDbConnectionString))
        {
            healthChecks.AddNpgSql(readDbConnectionString, name: "postgres-read", tags: ["ready"]);
        }

        var writeDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.WriteDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(writeDbConnectionString))
        {
            healthChecks.AddNpgSql(writeDbConnectionString, name: "postgres-write", tags: ["ready"]);
        }

        var rabbitMqConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.RabbitMqConnectionStringName);
        if (!string.IsNullOrEmpty(rabbitMqConnectionString))
        {
            healthChecks.AddRabbitMQ(
                async _ =>
                {
                    var factory = new ConnectionFactory { Uri = new Uri(rabbitMqConnectionString) };
                    return await factory.CreateConnectionAsync();
                },
                name: "rabbitmq",
                tags: ["ready"]);
        }

        return builder;
    }

    public static WebApplication UseInfrastructureEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
