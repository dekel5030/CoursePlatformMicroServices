using System.Security.Claims;
using CoursePlatform.ServiceDefaults;
using CoursePlatform.ServiceDefaults.Auth;
using Gateway.Api.Database;
using Kernel.Auth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yarp.ReverseProxy.Transforms;

namespace Gateway.Api;

public static class DependencyInjection
{
    internal const string AuthServiceName = "CoursePlatform.Auth";

    public static IHostApplicationBuilder AddGateway(this IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
        builder.Services.AddKeysDb(builder.Configuration);
        builder.Services.AddAuth();
        builder.Services.AddYarp(builder.Configuration);

        return builder;
    }

    public static WebApplication UseGatway(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapReverseProxy();

        return app;
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
            .SetApplicationName(AuthServiceName)
            .PersistKeysToDbContext<DataProtectionKeysContext>();

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
        })
            .AddCookie(IdentityConstants.ApplicationScheme, options =>
            {
                options.Cookie.Name = AuthServiceName;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddYarp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDiscovery();

        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddServiceDiscoveryDestinationResolver()
            .AddTransforms(builderContext =>
            {
                builderContext.AddRequestTransform(transformContext =>
                {
                    var user = transformContext.HttpContext.User;

                    if (user?.Identity?.IsAuthenticated == true)
                    {
                        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            transformContext.ProxyRequest.Headers.Add(HeaderNames.UserIdHeader, userId);
                        }

                        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

                        foreach (var role in roles)
                        {
                            transformContext.ProxyRequest.Headers.Add(HeaderNames.UserRoleHeader, role);
                        }

                        var permissions = user.FindAll(PermissionClaim.ClaimType).Select(c => c.Value);

                        foreach (var perrmission in permissions)
                        {
                            transformContext.ProxyRequest.Headers.Add(HeaderNames.UserPermissionsHeader, perrmission);
                        }
                    }

                    return ValueTask.CompletedTask;
                });
            });

        return services;
    }
}
