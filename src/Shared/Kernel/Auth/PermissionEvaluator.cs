using System.Security.Claims;
using Kernel.Auth.AuthTypes;

namespace Kernel.Auth;

public static class PermissionEvaluator
{
    public static bool HasPermission(ClaimsPrincipal user, ActionType action, ResourceType resource, string resourceId)
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
        string resourceId)
    {
        var patternsToCheck = new[]
        {
            (Action: action, ResourceId: resourceId),
            (Action: action, ResourceId: "*"),
            (Action: ActionType.Wildcard, ResourceId: resourceId),
            (Action: ActionType.Wildcard, ResourceId: "*")
        };

        foreach (var (checkAction, checkResourceId) in patternsToCheck)
        {
            var claimValue = PermissionClaim.Create(effect, checkAction, resource, checkResourceId).Value;

            if (user.HasClaim(PermissionClaim.ClaimType, claimValue))
            {
                return true;
            }
        }

        return false;
    }
}