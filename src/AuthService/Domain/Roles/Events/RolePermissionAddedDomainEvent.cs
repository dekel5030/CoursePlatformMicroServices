using Domain.Permissions;
using SharedKernel;

namespace Domain.Roles.Events;

public record RolePermissionAddedDomainEvent(Role Role, Permission Permission) : IDomainEvent;