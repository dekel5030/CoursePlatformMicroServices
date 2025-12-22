using Auth.Infrastructure.Auth.Extensions;
using Auth.Infrastructure.Database;
using Auth.Infrastructure.Extensions;
using Auth.Infrastructure.MassTransit;
using Auth.Infrastructure.Redis.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    internal const string WriteDbSectionName = "WriteDatabase";
    internal const string ReadDbSectionName = "ReadDatabase";
    internal const string RabbitMqSectionName = "RabbitMq";
    internal const string RedisConnectionName = "redis";

    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddInfrastructureDefaults();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.AddDistributedPermissionStore(RedisConnectionName);

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseInfrastructureDefaultEndpoints();
        app.UseAuth();

        return app;
    }

    private static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddAuthServices(configuration)
            .AddDatabase(configuration)
            .AddMassTransitInternal(configuration)
            .AddHealthChecksInternal(configuration);
    }

    private static IServiceCollection AddHealthChecksInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}