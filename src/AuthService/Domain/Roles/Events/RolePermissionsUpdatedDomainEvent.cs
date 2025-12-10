using Domain.Permissions;
using SharedKernel;

namespace Domain.Roles.Events;

public record RolePermissionsUpdatedDomainEvent(
    Role Role,
    IReadOnlyList<Permission> AddedPermissions,
    IReadOnlyList<Permission> RemovedPermissions) : IDomainEvent;
