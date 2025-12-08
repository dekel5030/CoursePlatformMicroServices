using Kernel.Auth.AuthTypes;

namespace Domain.Permissions;

public record RolePermission(ActionType Action, ResourceType Resource, ResourceId ResourceId)
{
    public RolePermission(ActionType action, ResourceType resource)
        : this(action, resource, ResourceId.Wildcard)
    {
    }

    public EffectType Effect => EffectType.Allow;
}