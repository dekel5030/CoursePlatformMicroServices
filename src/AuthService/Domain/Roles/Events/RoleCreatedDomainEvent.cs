using Auth.Domain.Roles;
using Kernel.Messaging.Abstractions;

namespace Auth.Domain.Roles.Events;

public record RoleCreatedDomainEvent(Role Role) : IDomainEvent;
