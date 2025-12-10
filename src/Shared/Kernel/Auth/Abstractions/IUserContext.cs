using Kernel.Auth.AuthTypes;

namespace Kernel.Auth.Abstractions;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    bool HasRole(RoleType role);
    bool HasPermission(ActionType action, ResourceType resource, ResourceId resourceId);
}
