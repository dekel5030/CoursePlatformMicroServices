using Kernel.Auth.AuthTypes;

namespace Domain.Permissions;

public record RolePermission(ResourceType Resource, ActionType Action, ResourceId ResourceId)
{
    public RolePermission(ResourceType resource, ActionType action)
        : this(resource, action, ResourceId.Wildcard)
    {
    }

    public EffectType Effect => EffectType.Allow;
}