using Auth.Application.Abstractions.Auth;
using Auth.Domain.AuthUsers;
using Auth.Domain.Permissions;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;

namespace Auth.Infrastructure.Auth;

public class PermissionResolver : IPermissionResolver
{
    public IEnumerable<string> ResolveEffectivePermissions(AuthUser user)
    {
        var allPermissions = user.Roles
            .SelectMany(role => role.Permissions)
            .Concat(user.Permissions)
            .Distinct()
            .ToList();

        List<Permission> flattened = FlattenHierarchy(allPermissions);
        List<Permission> resolved = ApplyDenyOverrides(flattened);

        return resolved.Select(p =>
            PermissionClaim.ToClaimValue(p.Effect, p.Action, p.Resource, p.ResourceId));
    }

    private static List<Permission> ApplyDenyOverrides(List<Permission> source)
    {
        var denyPermissions = source.Where(p => p.Effect == EffectType.Deny).ToList();

        IEnumerable<Permission> filteredAllowPermissions = source
            .Where(p => p.Effect == EffectType.Allow)
            .Where(allow => !denyPermissions.Any(deny => PermissionWiderThan(deny, allow)));

        return denyPermissions.Concat(filteredAllowPermissions).ToList();
    }

    private static List<Permission> FlattenHierarchy(List<Permission> source)
    {
        var result = new HashSet<Permission>(source);

        foreach (Permission parent in source)
        {
            foreach (Permission child in source)
            {
                if (parent != child &&
                    parent.Effect == child.Effect &&
                    PermissionWiderThan(parent, child))
                {
                    result.Remove(child);
                }
            }
        }

        return result.ToList();
    }

    private static bool PermissionWiderThan(Permission container, Permission target)
    {
        bool actionMatch = container.Action == ActionType.Wildcard || container.Action == target.Action;
        bool resourceMatch = container.Resource == ResourceType.Wildcard || container.Resource == target.Resource;
        bool idMatch = container.ResourceId == ResourceId.Wildcard || container.ResourceId == target.ResourceId;

        return actionMatch && resourceMatch && idMatch;
    }
}
