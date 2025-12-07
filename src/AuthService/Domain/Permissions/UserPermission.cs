using Kernel.Auth.AuthTypes;

namespace Domain.Permissions;

public record UserPermission(
    EffectType Effect, 
    ResourceType Resource, 
    ActionType Action, 
    ResourceId ResourceId);
