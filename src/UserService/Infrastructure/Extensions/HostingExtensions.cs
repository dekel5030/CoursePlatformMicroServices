using CoursePlatform.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions;

public static class HostingExtensions
{
    public static TBuilder AddInfrastructureDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddServiceDefaults();

        var connectionString = builder.Configuration.GetConnectionString(DependencyInjection.ReadDatabaseConnectionStringName);
        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.Services.AddHealthChecks()
                .AddNpgSql(connectionString, name: "postgres", tags: ["ready"]);
        }

        return builder;
    }

    public static WebApplication UseInfrastructureEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        return app;
    }
}
