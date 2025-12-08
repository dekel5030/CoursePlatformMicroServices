using Domain.Permissions;
using SharedKernel;

namespace Domain.Roles.Events;

public record RolePermissionAssignedDomainEvent(Role Role, Permission Permission) : IDomainEvent;