using Auth.Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Auth.Domain.AuthUsers.Events;

public record UserPermissionAddedDomainEvent(AuthUser User, Permission Permission) : IDomainEvent;
