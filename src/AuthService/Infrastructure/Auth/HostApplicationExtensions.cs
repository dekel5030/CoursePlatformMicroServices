using Application.Abstractions.Auth;
using Gateway.Api.Jwt;
using Infrastructure.Auth.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auth;

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

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection ConfigureKeycloakJwtAuth(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<KeycloakJwtOptions>(configuration.GetSection(KeycloakJwtOptions.SectionName));

        return services;
    }
}
