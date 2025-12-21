using Application.Abstractions.Auth;
using Gateway.Api.Jwt;
using Infrastructure.Auth.Context;
using Infrastructure.Auth.Jwt;
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
        services.Configure<InternalTokenOptions>(configuration.GetSection(InternalTokenOptions.SectionName));
        services.AddSingleton<KeyManager>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IPermissionResolver, PermissionResolver>();
        services.ConfigureKeycloakJwtAuth(configuration);

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
        });

        services.AddUserContext();

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

        return services;
    }

    private static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped<IExternalUserContext, ExternalUserContext>();

        return services;
    }
}
