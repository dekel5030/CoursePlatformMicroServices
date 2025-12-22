using Kernel.Auth.AuthTypes;

namespace Users.Application.Abstractions.Context;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    bool HasPermissionOnUsersResource(ActionType action, Guid resourceId);
    bool HasRole(RoleType role);
}