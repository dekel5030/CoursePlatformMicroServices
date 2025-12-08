using Domain.Permissions;
using SharedKernel;

namespace Domain.AuthUsers.Events;

public record UserPermissionRemovedDomainEvent(AuthUser User, Permission Permission) : IDomainEvent;
