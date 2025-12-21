using Application.Abstractions.Auth;
using CoursePlatform.ServiceDefaults.Auth;
using Infrastructure.Auth.Context;
using Infrastructure.Auth.Jwt;
using Infrastructure.Auth.Jwt.External;
using Infrastructure.Auth.Jwt.Internal;
using Kernel.Auth.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auth.Extensions;

internal static class HostApplicationExtensions
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<KeyManager>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IPermissionResolver, PermissionResolver>();

        services.ConfigureKeycloakJwtAuth(configuration);
        services.ConfigureInternalJwtAuth(configuration);

        services.AddAuthentication(AuthSchemes.Internal)
            .AddJwtBearer(AuthSchemes.Internal)
            .AddJwtBearer(AuthSchemes.Keycloak);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthSchemes.Keycloak, policy =>
            {
                policy.AddAuthenticationSchemes(AuthSchemes.Keycloak);
                policy.RequireAuthenticatedUser();
            });

            options.AddPolicy(AuthSchemes.Internal, policy =>
            {
                policy.AddAuthenticationSchemes(AuthSchemes.Internal);
                policy.RequireAuthenticatedUser();
            });

            options.DefaultPolicy = options.GetPolicy(AuthSchemes.Internal)!;
        });

        return services;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static IServiceCollection ConfigureKeycloakJwtAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KeycloakJwtOptions>(configuration.GetSection(KeycloakJwtOptions.SectionName));
        services.ConfigureOptions<KeycloakBearerOptionsSetup>();
        services.AddScoped<IExternalUserContext, ExternalUserContext>();

        return services;
    }

    private static IServiceCollection ConfigureInternalJwtAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<InternalTokenOptions>(configuration.GetSection(InternalTokenOptions.SectionName));
        services.ConfigureOptions<InternalJwtOptionsSetup>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}
