using Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Domain.AuthUsers.Events;

public record UserPermissionRemovedDomainEvent(AuthUser User, Permission Permission) : IDomainEvent;
