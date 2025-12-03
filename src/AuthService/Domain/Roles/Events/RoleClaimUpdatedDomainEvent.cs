using SharedKernel;

namespace Domain.Roles.Events;

public record RoleClaimUpdatedDomainEvent(Guid RoleId, string RoleName) : IDomainEvent;
