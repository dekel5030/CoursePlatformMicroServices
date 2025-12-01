using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();

        string connectionString = builder.Configuration.GetConnectionString(DependencyInjection.WriteDbSectionName)
            ?? throw new InvalidOperationException("Database connection string not found");

        builder.Services
            .AddHealthChecks()
            .AddNpgSql(connectionString, name: "authservice-db", tags: ["db", "postgres"]);

        return builder;
    }

    public static WebApplication UseInfrastructureEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
