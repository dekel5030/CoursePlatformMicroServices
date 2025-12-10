using Domain.Permissions;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public record UserPermissionAddedDomainEvent(AuthUser User, Permission Permission) : IDomainEvent;
