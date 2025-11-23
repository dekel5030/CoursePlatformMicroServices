using Application.Abstractions.Security;
using Infrastructure.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public class TokenService : ITokenService
{

    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;
    }

    public string GenerateAccessToken(TokenRequestDto request)
    {
        var claims = GetClaims(request);

        var rsa = RSA.Create();
        if (string.IsNullOrEmpty(_jwtOptions.PrivateKey))
        {
            throw new InvalidOperationException("JWT Private Key not configured");
        }
        
        rsa.ImportFromPem(_jwtOptions.PrivateKey);
        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa), 
            SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string HashRefreshToken(string refreshToken)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hashBytes);
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        try
        {
            Convert.FromBase64String(refreshToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static List<Claim> GetClaims(TokenRequestDto request)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, request.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        };

        // Add roles
        foreach (var role in request.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions
        foreach (var permission in request.Permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        return claims;
    }
}