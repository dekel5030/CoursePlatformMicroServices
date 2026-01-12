using Kernel.Auth;
using Kernel.Auth.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CoursePlatform.ServiceDefaults.Auth;

public static class AuthExtensions
{
    public const string InternalIssuer = "course-platform-auth";
    public const string InternalAudience = "course-platform-internal";

    public static IServiceCollection AddInternalAuth(
        this IServiceCollection services, 
        string authServiceUrl)
    {
        Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MetadataAddress = $"{authServiceUrl}/.well-known/openid-configuration";

                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = InternalIssuer,

                    ValidateAudience = true,
                    ValidAudience = InternalAudience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    NameClaimType = CoursePlatformClaims.UserId,
                    RoleClaimType = CoursePlatformClaims.Role,
                    ClockSkew = TimeSpan.Zero,
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        ILogger<JwtBearerHandler> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                        logger.LogError(context.Exception, "Internal Authentication failed.");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}
