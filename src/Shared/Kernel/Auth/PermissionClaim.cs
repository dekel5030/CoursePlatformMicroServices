using System.Security.Claims;
using Kernel.Auth.AuthTypes;

namespace Kernel.Auth;

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
public static partial class PermissionClaim
{
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
            ResourceId id)
    {
        string? value = ToClaimValue(effect, action, resource, id);

        return new Claim(CoursePlatformClaims.Permission, value);
    }

    /// <summary>
    /// Parses a structured permission claim string into a Claim object.
    /// </summary>
    /// <param name="claimValue">The claim value in the format: [Effect]:[Action]:[Resource]:[ID]</param>
    /// <returns>A Claim object representing the parsed permission.</returns>
    /// <exception cref="ArgumentException">Thrown if the claim format is invalid or any segment cannot be parsed.</exception>

    public static bool TryParse(string claimValue, out Claim result)
    {
        result = default!;

        if (string.IsNullOrWhiteSpace(claimValue))
        {
            return false;
        }

        string[] segments = claimValue.Split(':');

        if (segments.Length != 4)
        {
            return false;
        }

        string effectSegment = segments[0];
        string actionSegment = segments[1];
        string resourceSegment = segments[2];
        string idSegment = segments[3];

        if (!PermissionParser.TryParseEffect(effectSegment, out EffectType effect))
        {
            return false;
        }

        if (!PermissionParser.TryParseAction(actionSegment, out ActionType action))
        {
            return false;
        }

        if (!PermissionParser.TryParseResource(resourceSegment, out ResourceType resource))
        {
            return false;
        }

        var id = ResourceId.Create(idSegment);

        result = Create(effect, action, resource, id);
        return true;
    }

    /// <summary>
    /// Generates the raw string value for a permission claim.
    /// Format: effect:action:resource:id
    /// This is the Single Source of Truth for formatting.
    /// </summary>
    public static string ToClaimValue(
                EffectType effect,
                ActionType action,
                ResourceType resource,
                ResourceId id)
    {
        string effectString = effect.ToString().ToLowerInvariant();
        string actionString = action == ActionType.Wildcard ? "*" : action.ToString().ToLowerInvariant();
        string resourceString = resource == ResourceType.Wildcard ? "*" : resource.ToString().ToLowerInvariant();

        return $"{effectString}:{actionString}:{resourceString}:{id.Value}";
    }
}
