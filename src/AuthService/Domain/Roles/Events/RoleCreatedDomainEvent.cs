using Kernel.Messaging.Abstractions;

namespace Domain.Roles.Events;

public record RoleCreatedDomainEvent(Role Role) : IDomainEvent;
