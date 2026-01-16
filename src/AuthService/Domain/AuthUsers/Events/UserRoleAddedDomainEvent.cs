using Auth.Domain.Roles;
using Kernel.Messaging.Abstractions;

namespace Auth.Domain.AuthUsers.Events;

public record UserRoleAddedDomainEvent(AuthUser User, Role Role) : IDomainEvent;
