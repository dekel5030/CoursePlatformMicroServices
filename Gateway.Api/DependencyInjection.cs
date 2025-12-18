using CoursePlatform.ServiceDefaults;
using Gateway.Api.Jwt;
using Gateway.Api.Middleware;
using Gateway.Api.Services.AuthCacheRepository;
using Gateway.Api.Services.AuthSource;
using Gateway.Api.Services.UserEnrichmentService;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gateway.Api;

public static class DependencyInjection
{
    private const string RedisConnectionString = "redis";
    public const string AuthServiceName = "authservice";
    public static IHostApplicationBuilder AddGateway(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddRedisDistributedCache(RedisConnectionString);
        builder.Services.AddGatewayInternalServices();
        builder.Services.AddAuth(builder.Configuration);
        builder.Services.AddYarp(builder.Configuration);

        return builder;
    }

    public static WebApplication UseGatway(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<UserEnrichmentMiddleware>();

        app.MapReverseProxy();

        return app;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureJwtAuthentication(configuration);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.RequireHttpsMetadata = false);
        services.AddAuthorizationBuilder();

        return services;
    }

    private static IServiceCollection AddYarp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDiscovery();

        services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                .AddServiceDiscoveryDestinationResolver();
      
        return services;
    }

    private static IServiceCollection AddGatewayInternalServices(this IServiceCollection services)
    {
        services.AddTransient<IAuthCacheRepository, AuthRedisRepository>();
        services.AddTransient<IAuthSourceAdapter, AuthHttpAdapter>();

        services.AddTransient<IUserEnrichmentService, UserEnrichmentService>();

        services
            .AddHttpClient(AuthServiceName, client =>
            {
                client.BaseAddress = new Uri($"https://{AuthServiceName}");
                client.Timeout = TimeSpan.FromSeconds(50000);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("CoursePlatform-Gateway");
            })
            .AddStandardResilienceHandler(options =>
            {
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(600);
            });

        services.AddTransient<UserEnrichmentMiddleware>();

        return services;
    }
}
