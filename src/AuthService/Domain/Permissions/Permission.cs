using Domain.Permissions.Errors;
using Kernel;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;

namespace Domain.Permissions;

public record Permission(
    EffectType Effect, 
    ActionType Action, 
    ResourceType Resource, 
    ResourceId ResourceId)
{
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

    public override string ToString()
    {
        string effectString = Effect.ToString().ToLower();
        string actionString = Action.ToString().ToLower();
        string resourceString = Resource.ToString().ToLower();
        string resourceIdString = ResourceId.ToString();

        return $"{effectString}:{actionString}:{resourceString}:{resourceIdString}";
    }
};