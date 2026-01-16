using CoursePlatform.Contracts.AuthEvents;
using Kernel;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;
using Users.Domain.Users;
using Users.Domain.Users.Primitives;

namespace Users.Application.Users.Events;

internal sealed class UserRegisterdEventConsumer : IEventConsumer<UserRegisteredEvent>
{
    private readonly IWriteDbContext _dbContext;

    public UserRegisterdEventConsumer(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(
        UserRegisteredEvent message,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId { Value = message.UserId };
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken: cancellationToken);

        if (user is null)
        {
            var userName = new FullName(message.FirstName ?? string.Empty, message.LastName ?? string.Empty);
            Result<User> userCreationResult = User.CreateUser(userId, message.Email, userName);

            if (userCreationResult.IsSuccess)
            {
                _dbContext.Users.Add(userCreationResult.Value);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
