using Application.Abstractions.Messaging;
using Domain.Roles.Events;
using Infrastructure.Database;
using Kernel.Auth;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.SyncEventsHandlers.Roles;

internal class RolePermissionsUpdatedDomainEventHandler : IDomainEventHandler<RolePermissionsUpdatedDomainEvent>
{
    private readonly WriteDbContext _dbContext;

    public RolePermissionsUpdatedDomainEventHandler(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(
        RolePermissionsUpdatedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        var roleClaims = new List<IdentityRoleClaim<Guid>>();

        // Add new permission claims
        foreach (var permission in @event.AddedPermissions)
        {
            roleClaims.Add(new IdentityRoleClaim<Guid>
            {
                RoleId = @event.Role.Id,
                ClaimType = PermissionClaim.ClaimType,
                ClaimValue = PermissionClaim.Create(
                    permission.Effect,
                    permission.Action,
                    permission.Resource,
                    permission.ResourceId).Value
            });
        }

        if (roleClaims.Count > 0)
        {
            await _dbContext.RoleClaims.AddRangeAsync(roleClaims, cancellationToken);
        }

        // Remove permission claims
        if (@event.RemovedPermissions.Count > 0)
        {
            var claimValues = @event.RemovedPermissions
                .Select(p => PermissionClaim.Create(p.Effect, p.Action, p.Resource, p.ResourceId).Value)
                .ToList();

            var claimsToRemove = _dbContext.RoleClaims
                .Where(rc => rc.RoleId == @event.Role.Id 
                    && rc.ClaimType == PermissionClaim.ClaimType
                    && claimValues.Contains(rc.ClaimValue!))
                .ToList();

            _dbContext.RoleClaims.RemoveRange(claimsToRemove);
        }
    }
}
