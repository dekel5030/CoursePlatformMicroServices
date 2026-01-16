using Kernel.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Auth.Jwt.Internal;

internal class InternalJwtOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly InternalTokenOptions _jwtOptions;
    private readonly KeyManager _keyManager;

    public InternalJwtOptionsSetup(
        IOptions<InternalTokenOptions> jwtOptions,
        KeyManager keyManager)
    {
        _jwtOptions = jwtOptions.Value;
        _keyManager = keyManager;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != AuthSchemes.Internal)
        {
            return;
        }

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = _keyManager.GetPublicKey(),

            ClockSkew = TimeSpan.Zero,
            NameClaimType = CoursePlatformClaims.UserId,
            RoleClaimType = CoursePlatformClaims.Role
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(AuthSchemes.Internal, options);
}
