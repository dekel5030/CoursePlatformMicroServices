using Domain.Roles;
using Kernel.Messaging.Abstractions;

namespace Domain.AuthUsers.Events;

public record UserRoleRemovedDomainEvent(AuthUser User, Role Role) : IDomainEvent;
