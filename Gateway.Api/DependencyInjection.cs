using System.Security.Claims;
using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Auth;
using Gateway.Api.Jwt;
using Gateway.Api.Middleware;
using Gateway.Api.Services.PermissionsCache;
using Gateway.Api.Services.PermissionsSource;
using Gateway.Api.Services.UserPermissionsService;
using Kernel.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Yarp.ReverseProxy.Transforms;

namespace Gateway.Api;

public static class DependencyInjection
{
    public const string AuthServiceName = "authservice";
    public static IHostApplicationBuilder AddGateway(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
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
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
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
        services.AddTransient<IPermissionsCache, RedisPermissionsCache>();
        services.AddTransient<IPermissionsSource, AuthHttpPermissionsSource>();

        services.AddTransient<IUserPermissionsService, UserPermissionsService>();

        services
            .AddHttpClient(AuthServiceName, client =>
            {
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("CoursePlatform-Gateway");
            })
            .AddStandardResilienceHandler();

        return services;
    }
}
