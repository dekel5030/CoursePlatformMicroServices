using Kernel.AuthTypes;

namespace Application.Abstractions.Context;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    bool HasUserResourcePermission(ActionType action, Guid resourceId);
    bool HasRole(RoleType role);
}