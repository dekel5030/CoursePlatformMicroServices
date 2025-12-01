using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();

        string writeDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.WriteDbSectionName)
            ?? throw new InvalidOperationException("Write Database connection string not found");

        string readDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.ReadDbSectionName)
            ?? throw new InvalidOperationException("Read Database connection string not found");

        string rabbitMqConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.RabbitMqSectionName)
            ?? throw new InvalidOperationException("RabbitMQ connection string not found");

        builder.Services
            .AddHealthChecks()
            .AddNpgSql(writeDbConnectionString, name: "authservice-write-db", tags: ["ready", "db", "postgres"])
            .AddNpgSql(readDbConnectionString, name: "authservice-read-db", tags: ["ready", "db", "postgres"])
            .AddRabbitMQ(async _ =>
            {
                var factory = new ConnectionFactory { Uri = new Uri(rabbitMqConnectionString) };
                return await factory.CreateConnectionAsync();
            }, name: "authservice-rabbitmq", tags: ["ready", "rabbitmq"]);

        return builder;
    }

    public static WebApplication UseInfrastructureEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
