using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.SyncEventsHandlers.Users;

internal class UserRoleAddedDomainEventHandler : IDomainEventHandler<UserRoleAddedDomainEvent>
{
    private readonly WriteDbContext _dbContext;
    public UserRoleAddedDomainEventHandler(
        WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(UserRoleAddedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRoles.AddAsync(new IdentityUserRole<Guid>
        {
            RoleId = @event.Role.Id,
            UserId = @event.User.Id
        }, cancellationToken).AsTask();
    }
}
