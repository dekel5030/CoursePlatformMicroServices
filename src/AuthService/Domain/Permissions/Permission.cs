using System.Text.Json.Serialization;
using Auth.Domain.Permissions.Errors;
using Kernel;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;

namespace Auth.Domain.Permissions;

public record Permission
{
    public EffectType Effect { get; init; }
    public ActionType Action { get; init; }
    public ResourceType Resource { get; init; }
    public ResourceId ResourceId { get; init; }

    [JsonInclude]
    public string Key => $"{(int)Effect:D1}{(int)Action:D2}{(int)Resource:D3}{ResourceId.Value}";

    #pragma warning disable CS8618
    private Permission() { }
    #pragma warning restore CS8618 

    public Permission(
        EffectType effect,
        ActionType action,
        ResourceType resource,
        ResourceId resourceId)
    {
        Effect = effect;
        Action = action;
        Resource = resource;
        ResourceId = resourceId;
    }

    public static Permission CreateRolePermission(
        ActionType action,
        ResourceType resource,
        ResourceId resourceId)
    {
        return new Permission(EffectType.Allow, action, resource, resourceId);
    }

    public static Result<Permission> Parse(
        string effectSegment, 
        string actionSegment, 
        string resourceSegment, 
        string resourceIdSegment)
    {
        if (PermissionParser.TryParseEffect(effectSegment, out var effect) == false)
        {
            return Result<Permission>.Failure(PermissionErrors.InvalidEffect);
        }
        if (PermissionParser.TryParseAction(actionSegment, out var action) == false)
        {
            return Result<Permission>.Failure(PermissionErrors.InvalidAction);
        }
        if (PermissionParser.TryParseResource(resourceSegment, out var resource) == false)
        {
            return Result<Permission>.Failure(PermissionErrors.InvalidResource);
        }

        var resourceId = ResourceId.Create(resourceIdSegment);

        return Result<Permission>.Success(new Permission(effect, action, resource, resourceId));
    }

    public override string ToString()
    {
        return PermissionClaim.ToClaimValue(Effect, Action, Resource, ResourceId);
    }

    public bool Covers(Permission other)
    {
        if (Effect != other.Effect) return false;

        var actionMatches = Action == ActionType.Wildcard || Action == other.Action;

        var resourceMatches = Resource == ResourceType.Wildcard || Resource == other.Resource;

        var idMatches = ResourceId.IsWildcard || ResourceId.Value == other.ResourceId.Value;

        return actionMatches && resourceMatches && idMatches;
    }
};