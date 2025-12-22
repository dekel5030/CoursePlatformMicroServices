using Domain.Roles;
using Kernel.Messaging.Abstractions;

namespace Domain.AuthUsers.Events;

public record UserRoleAddedDomainEvent(AuthUser User, Role Role) : IDomainEvent;
