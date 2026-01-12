using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Application.Abstractions.Auth;
using Auth.Domain.AuthUsers;
using Auth.Domain.Roles;
using Kernel.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Auth.Jwt.Internal;

internal class TokenProvider : ITokenProvider
{
    private readonly KeyManager _keyManager;
    private readonly IOptions<InternalTokenOptions> _options;

    public TokenProvider(KeyManager keyManager, IOptions<InternalTokenOptions> options)
    {
        _keyManager = keyManager;
        _options = options;
    }

    public string GenerateToken(
        AuthUser user, 
        IEnumerable<string> effectivePermissions, 
        DateTime expiration)
    {
        RsaSecurityKey key = _keyManager.PrivateKey;
        key.KeyId = KeyManager.KeyId;
        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var claims = new List<Claim>
        {
            CoursePlatformClaims.CreateUserIdClaim(user.Id.Value),
            CoursePlatformClaims.CreateEmailClaim(user.Email.Address),
            CoursePlatformClaims.CreateFirstNameClaim(user.FullName.FirstName.Name),
            CoursePlatformClaims.CreateLastNameClaim(user.FullName.LastName.Name),
            CoursePlatformClaims.CreateIdentityIdClaim(user.IdentityId.ProviderId),
        };

        foreach (Role role in user.Roles)
        {
            claims.Add(CoursePlatformClaims.CreateRoleClaim(role.Name.Value));
        }

        foreach (var permission in effectivePermissions)
        {
            claims.Add(CoursePlatformClaims.CreatePermissionClaim(permission));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
