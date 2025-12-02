using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace CoursePlatform.ServiceDefaults.Auth;

public static class PermissionClaim
{
    public const string ClaimType = "cp_permission";

    public static Claim Create(
            EffectType effect,
            ActionType action,
            ResourceType resource,
            string id)
    {
        string effectString = effect.ToString().ToLowerInvariant();
        string actionString = action == ActionType.Wildcard ? "*" : action.ToString().ToLowerInvariant();
        string resourceString = resource == ResourceType.Wildcard ? "*" : resource.ToString().ToLowerInvariant();
        string idString = id == "*" ? "*" : id.ToLowerInvariant();
        
        return new Claim(ClaimType, $"{effectString}:{actionString}:{resourceString}:{idString}");
    }

    public static Claim Parse(string claimValue)
    {
        var segments = claimValue.Split(':');
        if (segments.Length != 4)
            throw new ArgumentException("Invalid permission claim format.");

        string effectSegment = segments[0];
        string actionSegment = segments[1];
        string resourceSegment = segments[2];
        string idSegment = segments[3];

        EffectType effect = ParseEffect(effectSegment);
        ActionType action = ParseAction(actionSegment);
        ResourceType resource = ParseResource(resourceSegment);
        string id = ParseId(idSegment);

        return Create(effect, action, resource, id);
    }

    public static Claim? TryParse(string claimValue)
    {
        try
        {
            return Parse(claimValue);
        }
        catch
        {
            return null;
        }
    }

    public static bool TryParse(
            string claimValue,
            [NotNullWhen(true)] out Claim? parsedClaim)
    {
        parsedClaim = null;

        try
        {
            parsedClaim = Parse(claimValue);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public enum EffectType
    {         
        Allow,
        Deny
    }

    public enum ActionType
    {
        Create,
        Read,
        Update,
        Delete,
        Wildcard
    }

    public enum ResourceType
    {
        Course,
        Lesson,
        User,
        Enrollment,
        Wildcard
    }

    private static EffectType ParseEffect(string effectSegment)
    {
        if (!Enum.TryParse(effectSegment, true, out EffectType effect))
            throw new ArgumentException("Invalid effect value.");

        return effect;
    }

    private static ActionType ParseAction(string actionSegment)
    {
        ActionType action;

        if (actionSegment == "*")
        {
            return ActionType.Wildcard;
        }
        else if (!Enum.TryParse(actionSegment, ignoreCase: true, out action))
        {
            throw new ArgumentException("Invalid action value.");
        }

        return action;
    }

    private static ResourceType ParseResource(string resourceSegment)
    {
        ResourceType resource;

        if (resourceSegment == "*")
        {
            return ResourceType.Wildcard;
        }
        else if (!Enum.TryParse(resourceSegment, ignoreCase: true, out resource))
        {
            throw new ArgumentException("Invalid resource value.");
        }

        return resource;
    }

    private static string ParseId(string idSegment)
    {
        if (string.IsNullOrWhiteSpace(idSegment))
        {
            throw new ArgumentException("Invalid id value.");
        }

        return idSegment;
    }
}
