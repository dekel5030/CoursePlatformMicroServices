using Domain.Permissions;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public record UserPermissionsUpdatedDomainEvent(
    AuthUser User, 
    IReadOnlyList<Permission> AddedPermissions,
    IReadOnlyList<Permission> RemovedPermissions) : IDomainEvent;
