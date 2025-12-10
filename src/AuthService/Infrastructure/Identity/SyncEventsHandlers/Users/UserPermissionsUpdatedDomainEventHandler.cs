using System.Security.Claims;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Infrastructure.Database;
using Kernel.Auth;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.SyncEventsHandlers.Users;

internal class UserPermissionsUpdatedDomainEventHandler : IDomainEventHandler<UserPermissionsUpdatedDomainEvent>
{
    private readonly WriteDbContext _dbContext;

    public UserPermissionsUpdatedDomainEventHandler(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(
        UserPermissionsUpdatedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        var userClaims = new List<IdentityUserClaim<Guid>>();

        // Add new permission claims
        foreach (var permission in @event.AddedPermissions)
        {
            Claim permissionClaim = PermissionClaim.Create(
                permission.Effect,
                permission.Action,
                permission.Resource,
                permission.ResourceId);

            userClaims.Add(new IdentityUserClaim<Guid>
            {
                UserId = @event.User.Id,
                ClaimType = PermissionClaim.ClaimType,
                ClaimValue = permissionClaim.Value,
            });
        }

        if (userClaims.Count > 0)
        {
            await _dbContext.UserClaims.AddRangeAsync(userClaims, cancellationToken);
        }

        // Remove permission claims
        if (@event.RemovedPermissions.Count > 0)
        {
            var claimValues = @event.RemovedPermissions
                .Select(p => PermissionClaim.Create(p.Effect, p.Action, p.Resource, p.ResourceId).Value)
                .ToList();

            var claimsToRemove = _dbContext.UserClaims
                .Where(uc => uc.UserId == @event.User.Id 
                    && uc.ClaimType == PermissionClaim.ClaimType
                    && claimValues.Contains(uc.ClaimValue!))
                .ToList();

            _dbContext.UserClaims.RemoveRange(claimsToRemove);
        }
    }
}
