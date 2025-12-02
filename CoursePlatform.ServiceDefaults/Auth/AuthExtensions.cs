using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CoursePlatform.ServiceDefaults.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddGatewayAuth(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = GatewayHeaderAuthenticationHandler.SchemeName;
            options.DefaultChallengeScheme = GatewayHeaderAuthenticationHandler.SchemeName;
        })
        .AddScheme<AuthenticationSchemeOptions, GatewayHeaderAuthenticationHandler>(
            GatewayHeaderAuthenticationHandler.SchemeName, null); 

        services.AddAuthorization();

        return services;
    }
}
