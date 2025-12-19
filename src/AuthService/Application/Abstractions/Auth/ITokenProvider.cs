using Domain.AuthUsers;

namespace Application.Abstractions.Auth;

public interface ITokenProvider
{
    string GenerateToken(
        AuthUser user,
        IEnumerable<string> effectivePermissions, 
        DateTime expiration);
}