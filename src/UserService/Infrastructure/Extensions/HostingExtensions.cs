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
        var healthChecks = builder.Services.AddHealthChecks();

        var writeDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection._writeDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(writeDbConnectionString))
        {
            healthChecks.AddNpgSql(
                writeDbConnectionString,
                name: "postgres-write",
                tags: ["ready"]);
        }

        var readDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection._readDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(readDbConnectionString) 
            && !string.Equals(readDbConnectionString, writeDbConnectionString))
        {
            healthChecks.AddNpgSql(
                readDbConnectionString,
                name: "postgres-read",
                tags: ["ready"]);
        }

        return builder;
    }

    public static WebApplication UseInfrastructureDefaultEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
