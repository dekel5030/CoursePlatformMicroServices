using Domain.AuthUsers;

namespace Application.Abstractions.Auth;

public interface ITokenProvider
{
    string GenerateInternalToken(
        AuthUser user,
        IEnumerable<string> permissions, 
        DateTime expiration);
}