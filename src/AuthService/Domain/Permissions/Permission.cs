using Domain.Permissions.Errors;
using Kernel;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;
using SharedKernel;

namespace Domain.Permissions;

public class Permission : Entity
{
    public Guid Id { get; private set; }
    public EffectType Effect { get; private set; }
    public ActionType Action { get; private set; }
    public ResourceType Resource { get; private set; }
    public ResourceId ResourceId { get; private set; }

    private Permission() 
    {
        ResourceId = ResourceId.Wildcard;
    }

    private Permission(
        EffectType effect,
        ActionType action,
        ResourceType resource,
        ResourceId resourceId)
    {
        Id = Guid.CreateVersion7();
        Effect = effect;
        Action = action;
        Resource = resource;
        ResourceId = resourceId;
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

        ResourceId resourceId = ResourceId.Create(resourceIdSegment);

        return Result<Permission>.Success(new Permission(effect, action, resource, resourceId));
    }

    public static Permission CreateForRole(
        ActionType action,
        ResourceType resource,
        ResourceId resourceId)
    {
        return new Permission(EffectType.Allow, action, resource, resourceId);
    }

    // Override Equals and GetHashCode to compare by business values
    // This maintains the value object semantics for domain logic
    public override bool Equals(object? obj)
    {
        if (obj is not Permission other)
        {
            return false;
        }

        return Effect == other.Effect
            && Action == other.Action
            && Resource == other.Resource
            && ResourceId.Equals(other.ResourceId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Effect, Action, Resource, ResourceId);
    }
}