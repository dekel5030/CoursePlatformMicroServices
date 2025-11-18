using System.Security.Cryptography;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Web.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind JWT configuration to JwtOptions
        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        // Load keys from files if file paths are provided
        if (!string.IsNullOrEmpty(jwtOptions.PrivateKeyFile))
        {
            var privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), jwtOptions.PrivateKeyFile);
            if (File.Exists(privateKeyPath))
            {
                jwtOptions.PrivateKey = File.ReadAllText(privateKeyPath);
            }
            else
            {
                throw new InvalidOperationException($"Private key file not found: {privateKeyPath}");
            }
        }

        if (!string.IsNullOrEmpty(jwtOptions.PublicKeyFile))
        {
            var publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), jwtOptions.PublicKeyFile);
            if (File.Exists(publicKeyPath))
            {
                jwtOptions.PublicKey = File.ReadAllText(publicKeyPath);
            }
            else
            {
                throw new InvalidOperationException($"Public key file not found: {publicKeyPath}");
            }
        }

        // Validate that keys are configured
        if (string.IsNullOrEmpty(jwtOptions.PublicKey))
        {
            throw new InvalidOperationException("JWT Public Key not configured. Provide either PublicKey or PublicKeyFile in configuration.");
        }

        if (string.IsNullOrEmpty(jwtOptions.Issuer))
        {
            throw new InvalidOperationException("JWT Issuer not configured");
        }

        if (string.IsNullOrEmpty(jwtOptions.Audience))
        {
            throw new InvalidOperationException("JWT Audience not configured");
        }

        // Register JwtOptions as singleton in DI
        services.AddSingleton(jwtOptions);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Use RSA public key for validation
            var rsa = RSA.Create();
            rsa.ImportFromPem(jwtOptions.PublicKey);

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ClockSkew = TimeSpan.Zero
            };

            // Configure to read JWT from cookies
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Try to get token from cookie first
                    var accessToken = context.Request.Cookies["accessToken"];
                    
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    // If not in cookie, fall back to Authorization header (default behavior)
                    
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
