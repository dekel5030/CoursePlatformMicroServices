using Kernel.Auth.AuthTypes;

namespace Kernel.Auth.Abstractions;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool HasRole(RoleType role);
    bool HasPermission(ActionType action, ResourceType resource, ResourceId resourceId);
}
