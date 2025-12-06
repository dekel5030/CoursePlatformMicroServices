using SharedKernel;

namespace Domain.Roles.Events;

public record RoleUpsertedDomainEvent(Role role) : IDomainEvent;