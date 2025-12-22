using Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Domain.Roles.Events;

public record RolePermissionRemovedDomainEvent(Role Role, Permission Permission) : IDomainEvent;