using Auth.Domain.AuthUsers;
using Auth.Domain.Permissions;
using Kernel.Messaging.Abstractions;

namespace Auth.Domain.AuthUsers.Events;

public record UserPermissionRemovedDomainEvent(AuthUser User, Permission Permission) : IDomainEvent;
