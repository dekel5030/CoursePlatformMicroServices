using SharedKernel;

namespace Domain.Roles.Events;

public record RoleCreatedDomainEvent(Guid RoleId, string RoleName) : IDomainEvent;
