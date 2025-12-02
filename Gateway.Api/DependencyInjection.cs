using Gateway.Api.Database;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Api;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddGateway(this IHostApplicationBuilder builder)
    {
        builder.Services.AddKeysDb(builder.Configuration);

        return builder;
    }
    private static IServiceCollection AddKeysDb(this IServiceCollection services, IConfiguration configuration)
    {
        var authDbConnectionString = configuration.GetConnectionString("authdb");

        services.AddDbContext<DataProtectionKeysContext>(options =>
        {
            options
                .UseNpgsql(authDbConnectionString)
                .UseSnakeCaseNamingConvention();

        });

        services.AddDataProtection()
            .SetApplicationName("CoursePlatform.Auth")
            .PersistKeysToDbContext<DataProtectionKeysContext>();

        return services;
    }
}
