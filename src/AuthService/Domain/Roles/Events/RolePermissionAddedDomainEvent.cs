using Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Domain.Roles.Events;

public record RolePermissionAddedDomainEvent(Role Role, Permission Permission) : IDomainEvent;