using SharedKernel;

namespace Domain.Roles.Events;

public record RolePermissionAssignedDomainEvent(Role Role) : IDomainEvent;