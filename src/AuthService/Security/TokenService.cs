using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Dtos;
using Common.Auth;
using Common.Auth.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Security;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(TokenRequestDto request)
    {
        var claims = GetClaims(request);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _jwtOptions.Key));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static List<Claim> GetClaims(TokenRequestDto request)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, request.Email),
            new Claim(JwtRegisteredClaimNames.Name, request.FullName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        };

        var permissionClaims = request.Permissions
            .Select(p => new Claim(CustomClaimNames.Permission, p.Name));

        claims.AddRange(permissionClaims);

        return claims;
    }
}