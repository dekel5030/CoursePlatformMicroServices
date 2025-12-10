using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Infrastructure.Database;
using Kernel.Auth;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.SyncEventsHandlers.Roles;

internal class RolePermissionAssignedDomainEventHandler : IDomainEventHandler<RolePermissionAddedDomainEvent>
{
    private readonly WriteDbContext _dbContext;

    public RolePermissionAssignedDomainEventHandler(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(RolePermissionAddedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var claim = new IdentityRoleClaim<Guid>
        {
            RoleId = @event.Role.Id,
            ClaimType = PermissionClaim.ClaimType,
            ClaimValue = PermissionClaim.Create(
                    @event.Permission.Effect,
                    @event.Permission.Action,
                    @event.Permission.Resource,
                    @event.Permission.ResourceId).Value
        };

        return _dbContext.RoleClaims.AddAsync(claim, cancellationToken).AsTask();
    }
}
