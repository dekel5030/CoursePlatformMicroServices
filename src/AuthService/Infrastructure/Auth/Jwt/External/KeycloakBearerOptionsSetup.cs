using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Auth.Jwt.External;

internal class KeycloakBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly KeycloakJwtOptions _options;

    public KeycloakBearerOptionsSetup(IOptions<KeycloakJwtOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != AuthSchemes.Keycloak)
        {
            return;
        }

        options.Authority = _options.Authority;
        options.Audience = _options.Audience;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public void Configure(JwtBearerOptions options)
    {
    }
}
