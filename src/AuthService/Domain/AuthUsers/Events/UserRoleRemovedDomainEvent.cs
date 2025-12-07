using Domain.Roles;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public record UserRoleRemovedDomainEvent(AuthUser User, Role Role) : IDomainEvent;
