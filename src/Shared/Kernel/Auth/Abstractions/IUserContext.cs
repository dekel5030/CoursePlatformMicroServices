using Kernel.Auth.AuthTypes;

namespace Kernel.Auth.Abstractions;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? UserEmail { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool HasRole(RoleType role);
    bool HasPermission(ActionType action, ResourceType resource, ResourceId resourceId);
}
