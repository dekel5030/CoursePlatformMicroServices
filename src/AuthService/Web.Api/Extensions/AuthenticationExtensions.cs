using Domain.AuthUsers;
using Domain.Roles;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace Auth.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(IdentityConstants.ApplicationScheme)
        .AddJwtBearer();

        services.AddIdentity<AuthUser, Role>()
            .AddEntityFrameworkStores<AuthDbContext>();

        services.AddAuthorization();

        return services;
    }
}