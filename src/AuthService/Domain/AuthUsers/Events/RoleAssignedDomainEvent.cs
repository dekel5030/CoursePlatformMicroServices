using Domain.AuthUsers.Primitives;
using Domain.Roles.Primitives;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public sealed record RoleAssignedDomainEvent(
    AuthUserId AuthUserId,
    RoleId RoleId,
    string RoleName,
    DateTime AssignedAt
) : IDomainEvent;
