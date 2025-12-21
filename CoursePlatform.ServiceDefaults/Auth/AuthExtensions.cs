using Kernel.Auth.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CoursePlatform.ServiceDefaults.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddInternalAuth(this IServiceCollection services, string authority, string audience)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // הכתובת של ה-AuthService (ממנה הוא מושך את ה-Well-known/JWKS)
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = false; // בשביבת פיתוח פנימית

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // מיפוי נכון של Claims לזהויות ASP.NET
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}