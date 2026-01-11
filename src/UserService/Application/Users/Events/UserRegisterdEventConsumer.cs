using CoursePlatform.Contracts.AuthEvents;
using Domain.Users;
using Domain.Users.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;

namespace Users.Application.Users.Events;

internal sealed class UserRegisterdEventConsumer : IEventConsumer<UserRegisteredEvent>
{
    private readonly IWriteDbContext _dbContext;

    public UserRegisterdEventConsumer(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(
        UserRegisteredEvent @event, 
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId { Value = @event.UserId };
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken: cancellationToken);

        if (user is null)
        {
            var userName = new FullName(@event.FirstName ?? string.Empty, @event.LastName ?? string.Empty);
            var userCreationResult = User.CreateUser(userId, @event.Email, userName);

            if (userCreationResult.IsSuccess)
            {
                _dbContext.Users.Add(userCreationResult.Value);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }    
        }
    }
}
