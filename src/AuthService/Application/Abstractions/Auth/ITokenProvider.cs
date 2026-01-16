using Auth.Domain.AuthUsers;

namespace Auth.Application.Abstractions.Auth;

public interface ITokenProvider
{
    string GenerateToken(
        AuthUser user,
        IEnumerable<string> effectivePermissions,
        DateTime expiration);
}