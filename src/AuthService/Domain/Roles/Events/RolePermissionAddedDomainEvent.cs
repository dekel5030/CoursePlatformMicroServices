using Auth.Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Auth.Domain.Roles.Events;

public record RolePermissionAddedDomainEvent(Role Role, Permission Permission) : IDomainEvent;