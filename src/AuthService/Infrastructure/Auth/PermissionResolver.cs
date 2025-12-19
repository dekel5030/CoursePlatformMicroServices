using Application.Abstractions.Auth;
using Domain.AuthUsers;
using Domain.Permissions;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;

namespace Infrastructure.Auth;
public class PermissionResolver : IPermissionResolver
{
    public IEnumerable<string> ResolveEffectivePermissions(AuthUser user)
    {
        var allPermissions = user.Roles
            .SelectMany(role => role.Permissions)
            .Concat(user.Permissions)
            .Distinct()
            .ToList();

        var flattened = FlattenHierarchy(allPermissions);
        var resolved = ApplyDenyOverrides(flattened);

        return resolved.Select(p =>
            PermissionClaim.ToClaimValue(p.Effect, p.Action, p.Resource, p.ResourceId));
    }

    private static List<Permission> ApplyDenyOverrides(List<Permission> source)
    {
        var denyPermissions = source.Where(p => p.Effect == EffectType.Deny).ToList();
        var allowPermissions = source.Where(p => p.Effect == EffectType.Allow).ToHashSet();

        foreach (var denyPerm in  denyPermissions)
        {
            foreach (var allowPerm in allowPermissions.ToList())
            {
                if (PermissionWiderThan(denyPerm, allowPerm))
                {
                    allowPermissions.Remove(allowPerm);
                }
            }
        }

        return denyPermissions.Concat(allowPermissions).ToList();
    }

    private static List<Permission> FlattenHierarchy(List<Permission> source)
    {
        var result = new HashSet<Permission>(source);

        foreach (var parent in source)
        {
            foreach (var child in source)
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