using Domain.Roles;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public record UserRoleAddedDomainEvent(AuthUser User, Role Role) : IDomainEvent;
