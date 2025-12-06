using SharedKernel;

namespace Domain.Roles.Events;

public record RoleUpsertedDomainEvent(Guid RoleId, string RoleName) : IDomainEvent;