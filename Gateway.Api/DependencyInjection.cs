using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Swagger;
using Gateway.Api.Jwt;
using Gateway.Api.Middleware;
using Gateway.Api.Services.AuthClient;
using Gateway.Api.Services.CacheService;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gateway.Api;

internal static class DependencyInjection
{
    private const string RedisConnectionString = "redis";
    public const string AuthServiceName = "authservice";
    public static IHostApplicationBuilder AddGateway(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.AddRedisDistributedCache(RedisConnectionString);
        builder.Services.AddSingleton<ICacheService, RedisCache>();
        builder.Services.AddGatewayInternalServices();
        builder.Services.AddAuth(builder.Configuration);
        builder.Services.AddYarp(builder.Configuration);
        SwaggerExtensions.AddDefaultOpenApi<IHostApplicationBuilder>(builder, JwtBearerDefaults.AuthenticationScheme);
        return builder;
    }

    public static WebApplication UseGatway(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api/auth/swagger/v1/swagger.json", "Auth Service API");
            });
        }

        app.UseMiddleware<UserEnrichmentMiddleware>();

        app.MapReverseProxy();

        return app;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureKeycloakAuthentication(configuration);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.RequireHttpsMetadata = false);
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
        services.AddTransient<IAuthClient, AuthHttpClient>();

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
