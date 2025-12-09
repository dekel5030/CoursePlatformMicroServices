using System.Security.Claims;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Infrastructure.Database;
using Kernel.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.SyncEventsHandlers.Users;

internal class UserPermissionRemovedDomainEventHandler
        : IDomainEventHandler<UserPermissionRemovedDomainEvent>
{
    private readonly WriteDbContext _dbContext;

    public UserPermissionRemovedDomainEventHandler(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(
        UserPermissionRemovedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        Claim permissionClaim = PermissionClaim.Create(
            @event.Permission.Effect,
            @event.Permission.Action,
            @event.Permission.Resource,
            @event.Permission.ResourceId);

        IdentityUserClaim<Guid> userClaim = new()
        {
            UserId = @event.User.Id,
            ClaimType = PermissionClaim.ClaimType,
            ClaimValue = permissionClaim.Value,
        };

        return _dbContext.UserClaims
            .Where(
                ur => 
                    ur.UserId == userClaim.UserId && 
                    ur.ClaimValue == userClaim.ClaimValue &&
                    ur.ClaimType == userClaim.ClaimType)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
