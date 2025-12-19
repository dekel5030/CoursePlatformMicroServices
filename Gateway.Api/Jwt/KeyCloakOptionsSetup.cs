using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Api.Jwt;

public class KeyCloakOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly KeycloakJwtOptions _jwtOptions;

    public KeyCloakOptionsSetup(IOptions<KeycloakJwtOptions> options)
    {
        _jwtOptions = options.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.Authority = _jwtOptions.Authority;
        options.Audience = _jwtOptions.Audience;

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
        return;
    }
}