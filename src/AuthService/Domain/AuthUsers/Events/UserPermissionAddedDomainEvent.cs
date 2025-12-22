using Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Domain.AuthUsers.Events;

public record UserPermissionAddedDomainEvent(AuthUser User, Permission Permission) : IDomainEvent;
