using System.Security.Claims;
using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Infrastructure.Database;
using Kernel.Auth;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.SyncEventsHandlers.Users;

internal class UserPermissionAddedDomainEventHandler : IDomainEventHandler<UserPermissionAddedDomainEvent>
{
    private readonly WriteDbContext _dbContext;

    public UserPermissionAddedDomainEventHandler(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(
        UserPermissionAddedDomainEvent @event, 
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

        return _dbContext.UserClaims.AddAsync(userClaim, cancellationToken).AsTask();
    }
}
