using SharedKernel;

namespace Domain.Roles.Events;

public record RoleCreatedDomainEvent(Role Role) : IDomainEvent;
