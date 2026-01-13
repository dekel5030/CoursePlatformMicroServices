using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Users.Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();
        IHealthChecksBuilder healthChecks = builder.Services.AddHealthChecks();

        string? writeDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.WriteDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(writeDbConnectionString))
        {
            healthChecks.AddNpgSql(
                writeDbConnectionString,
                name: "postgres-write",
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(3));
        }

        string? readDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.ReadDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(readDbConnectionString)
            && !string.Equals(readDbConnectionString, writeDbConnectionString, StringComparison.Ordinal))
        {
            healthChecks.AddNpgSql(
                readDbConnectionString,
                name: "postgres-read",
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(3));
        }

        return builder;
    }

    public static WebApplication UseInfrastructureDefaultEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
