using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions;

public static class HostingExtensions
{
    private const string WriteDatabaseSectionName = "WriteDatabase";

    public static TBuilder AddInfrastructureServiceDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();

        string connectionString = builder.Configuration.GetConnectionString(WriteDatabaseSectionName)
            ?? throw new InvalidOperationException(
                $"Connection string '{WriteDatabaseSectionName}' not found in configuration.");

        builder.Services.AddHealthChecks()
            .AddNpgSql(connectionString, name: "postgres", tags: ["db", "postgres"]);

        return builder;
    }

    public static WebApplication UseInfrastructureEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
