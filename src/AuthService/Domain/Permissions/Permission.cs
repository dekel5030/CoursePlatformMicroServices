using Domain.Permissions.Errors;
using Kernel;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;

namespace Domain.Permissions;

public class Permission : IEquatable<Permission>
{
    public int Id { get; private set; }
    public EffectType Effect { get; private set; }
    public ActionType Action { get; private set; }
    public ResourceType Resource { get; private set; }
    public ResourceId ResourceId { get; private set; }

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
            return Result<Permission>.Failure(PermissionErrors.InvalidAction);
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

    public bool Equals(Permission? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Action == other.Action &&
               Resource == other.Resource &&
               ResourceId == other.ResourceId &&
               Effect == other.Effect;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Permission)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Action, Resource, ResourceId, Effect);
    }

    public override string ToString()
    {
        return PermissionClaim.ToClaimValue(Effect, Action, Resource, ResourceId);
    }
};