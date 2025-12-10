using Domain.Permissions.Errors;
using Kernel;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;
using SharedKernel;

namespace Domain.Permissions;

public class Permission : Entity, IEquatable<Permission>
{
    public Guid Id { get; private set; }
    public EffectType Effect { get; private set; }
    public ActionType Action { get; private set; }
    public ResourceType Resource { get; private set; }
    public ResourceId ResourceId { get; private set; } = null!;

    // EF Core requires a parameterless constructor
    private Permission() { }

    public Permission(
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

    // Implement value-based equality to maintain business logic behavior
    // Two permissions are equal if they have the same Effect, Action, Resource, and ResourceId
    public bool Equals(Permission? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Effect == other.Effect
            && Action == other.Action
            && Resource == other.Resource
            && ResourceId.Equals(other.ResourceId);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Permission);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Effect, Action, Resource, ResourceId);
    }
};