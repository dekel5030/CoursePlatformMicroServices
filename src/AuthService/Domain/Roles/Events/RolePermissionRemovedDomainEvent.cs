using SharedKernel;

namespace Domain.Roles.Events;

public record RolePermissionRemovedDomainEvent(Role Role) : IDomainEvent;