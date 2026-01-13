using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Courses.Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();
        IHealthChecksBuilder healthChecks = builder.Services.AddHealthChecks();

        string writeDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.WriteDatabaseConnectionSection);
        if (!string.IsNullOrEmpty(writeDbConnectionString))
        {
            healthChecks.AddNpgSql(
                writeDbConnectionString,
                name: "postgres-write",
                tags: ["ready"],
                timeout: TimeSpan.FromSeconds(3));
        }

        string readDbConnectionString = builder.Configuration.GetConnectionString(DependencyInjection.ReadDatabaseConnectionSection);
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
