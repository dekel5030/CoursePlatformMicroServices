using Auth.Domain.AuthUsers;

namespace Auth.Application.Abstractions.Auth;

public interface IPermissionResolver
{
    IEnumerable<string> ResolveEffectivePermissions(AuthUser user);
}