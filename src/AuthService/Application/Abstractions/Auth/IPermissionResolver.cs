using Domain.AuthUsers;

namespace Application.Abstractions.Auth;

public interface IPermissionResolver
{
    IEnumerable<string> ResolveEffectivePermissions(AuthUser user);
}