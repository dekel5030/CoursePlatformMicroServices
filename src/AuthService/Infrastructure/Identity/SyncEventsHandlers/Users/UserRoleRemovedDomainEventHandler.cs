using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.SyncEventsHandlers.Users;

internal class UserRoleRemovedDomainEventHandler : IDomainEventHandler<UserRoleRemovedDomainEvent>
{
    private readonly WriteDbContext _dbContext;

    public UserRoleRemovedDomainEventHandler(
        WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(
        UserRoleRemovedDomainEvent @event, 
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRoles
            .Where(ur => ur.UserId == @event.User.Id && ur.RoleId == @event.Role.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
