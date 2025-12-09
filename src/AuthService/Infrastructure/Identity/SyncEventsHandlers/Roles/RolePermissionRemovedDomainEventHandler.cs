using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Infrastructure.Database;
using Kernel.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.SyncEventsHandlers.Roles;

internal class RolePermissionRemovedDomainEventHandler : IDomainEventHandler<RolePermissionRemovedDomainEvent>
{
    private readonly WriteDbContext _dbContext;
    public RolePermissionRemovedDomainEventHandler(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task Handle(RolePermissionRemovedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var expectedValue = PermissionClaim
            .ToClaimValue(
                @event.Permission.Effect, 
                @event.Permission.Action, 
                @event.Permission.Resource, 
                @event.Permission.ResourceId);

        return _dbContext.RoleClaims
            .Where(
                rc => rc.RoleId == @event.Role.Id &&
                rc.ClaimType == PermissionClaim.ClaimType &&
                rc.ClaimValue == expectedValue)
            .ExecuteDeleteAsync(cancellationToken);
    }
}