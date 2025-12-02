using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace CoursePlatform.ServiceDefaults.Auth;

/// <summary>
/// A static factory for creating and parsing structured permission claims 
/// in the format: [Effect]:[Action]:[Resource Type]:[Resource ID].
/// <para>
/// Example usage:
/// <code>
/// var claim = PermissionClaim.Create(
///     PermissionClaim.EffectType.Allow,
///     PermissionClaim.ActionType.Read,
///     PermissionClaim.ResourceType.Course,
///     "123");
/// </code>
/// </para>
/// </summary>/// <summary>
/// A static factory for creating and parsing structured permission claims 
/// in the format: [Effect]:[Action]:[Resource Type]:[Resource ID].
/// </summary>
public static class PermissionClaim
{
    public const string ClaimType = "cp_permission";


    /// <summary>
    /// Creates a new permission claim string with all required segments (Effect, Action, Resource, ID).
    /// </summary>
    /// <param name="effect">The effect type (Allow or Deny).</param>
    /// <param name="action">The action being permitted (e.g., Read, Update, Wildcard).</param>
    /// <param name="resource">The resource type (e.g., Course, User).</param>
    /// <param name="id">The specific resource ID or '*' for all instances.</param>
    /// <returns>A structured Claim object.</returns>
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

    /// <summary>
    /// Parses a structured permission claim string into a Claim object.
    /// </summary>
    /// <param name="claimValue">The claim value in the format: [Effect]:[Action]:[Resource]:[ID]</param>
    /// <returns>A Claim object representing the parsed permission.</returns>
    /// <exception cref="ArgumentException">Thrown if the claim format is invalid or any segment cannot be parsed.</exception>

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

    /// <summary>
    /// Attempts to parse a permission claim string into a Claim object.
    /// Returns null if parsing fails.
    /// </summary>
    /// <param name="claimValue">The claim value to parse.</param>
    /// <returns>A Claim object if parsing succeeds; otherwise, null.</returns>

    /// <summary>
    /// Attempts to parse a permission claim string into a Claim object using an out parameter.
    /// </summary>
    /// <param name="claimValue">The claim value to parse.</param>
    /// <param name="parsedClaim">The parsed Claim if successful; otherwise null.</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>

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

    /// <summary>
    /// The effect type of a permission: either Allow or Deny.
    /// </summary>
    public enum EffectType
    {
        /// <summary>Permission is allowed.</summary>
        Allow,
        /// <summary>Permission is denied.</summary>
        Deny
    }

    /// <summary>
    /// Actions that can be performed on a resource.
    /// </summary>
    public enum ActionType
    {
        /// <summary>Create a resource.</summary>
        Create,
        /// <summary>Read a resource.</summary>
        Read,
        /// <summary>Update a resource.</summary>
        Update,
        /// <summary>Delete a resource.</summary>
        Delete,
        /// <summary>Wildcard for all actions.</summary>
        Wildcard
    }

    /// <summary>
    /// Resources that can be accessed.
    /// </summary>
    public enum ResourceType
    {
        Course,
        Lesson,
        User,
        Enrollment,
        /// <summary>Wildcard for all resources.</summary>
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
