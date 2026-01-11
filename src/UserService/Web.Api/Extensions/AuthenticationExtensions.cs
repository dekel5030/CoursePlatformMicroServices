using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Users.Api.Extensions;

internal static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        services.AddAuthorization();

        return services;
    }
}