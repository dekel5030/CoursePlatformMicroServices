using Domain.Permissions;
using SharedKernel;

namespace Domain.Roles.Events;

public record RolePermissionRemovedDomainEvent(Role Role, RolePermission Permission) : IDomainEvent;