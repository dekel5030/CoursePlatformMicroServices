using System.Security.Claims;
using Kernel.Auth.AuthTypes;

namespace Kernel.Auth;

public static class PermissionEvaluator
{
    public static bool HasPermission(ClaimsPrincipal user, ActionType action, ResourceType resource, ResourceId resourceId)
    {
        if (CheckRule(user, EffectType.Deny, action, resource, resourceId))
        {
            return false;
        }

        if (CheckRule(user, EffectType.Allow, action, resource, resourceId))
        {
            return true;
        }

        return false;
    }

    private static bool CheckRule(
        ClaimsPrincipal user, 
        EffectType effect, 
        ActionType action, 
        ResourceType resource, 
        ResourceId resourceId)
    {
        var patternsToCheck = new[] {
            (Action: action, Resource: resource, ResourceId: resourceId),
            (Action: action, Resource: resource, ResourceId: ResourceId.Wildcard),
            (Action: action, Resource: ResourceType.Wildcard, ResourceId: resourceId), 
            (Action: action, Resource: ResourceType.Wildcard, ResourceId: ResourceId.Wildcard),

            (Action: ActionType.Wildcard, Resource: resource, ResourceId: resourceId),
            (Action: ActionType.Wildcard, Resource: resource, ResourceId: ResourceId.Wildcard),
            (Action: ActionType.Wildcard, Resource: ResourceType.Wildcard, ResourceId: resourceId),
            (Action: ActionType.Wildcard, Resource: ResourceType.Wildcard, ResourceId: ResourceId.Wildcard)
        };

        foreach (var (checkAction, checkResource, checkResourceId) in patternsToCheck)
        {
            var claimValue = PermissionClaim.ToClaimValue(effect, checkAction, checkResource, checkResourceId);

            if (user.HasClaim(CoursePlatformClaims.Permission, claimValue))
            {
                return true;
            }
        }

        return false;
    }
}