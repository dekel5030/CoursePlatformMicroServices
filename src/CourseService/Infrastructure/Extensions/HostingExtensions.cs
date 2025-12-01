using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureServiceDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();

        string writeDatabaseConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.WriteDatabaseConnectionName)
            ?? throw new InvalidOperationException(
                $"Connection string '{DependencyInjection.WriteDatabaseConnectionName}' not found in configuration.");

        string readDatabaseConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.ReadDatabaseConnectionName)
            ?? throw new InvalidOperationException(
                $"Connection string '{DependencyInjection.ReadDatabaseConnectionName}' not found in configuration.");

        string rabbitMqConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.RabbitMqConnectionName)
            ?? throw new InvalidOperationException(
                $"Connection string '{DependencyInjection.RabbitMqConnectionName}' not found in configuration.");

        builder.Services.AddHealthChecks()
            .AddNpgSql(writeDatabaseConnectionString, name: "postgres-write", tags: ["ready"])
            .AddNpgSql(readDatabaseConnectionString, name: "postgres-read", tags: ["ready"])
            .AddRabbitMQ(_ =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(rabbitMqConnectionString)
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            }, name: "rabbitmq", tags: ["ready"]);

        return builder;
    }

    public static WebApplication UseInfrastructureEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
