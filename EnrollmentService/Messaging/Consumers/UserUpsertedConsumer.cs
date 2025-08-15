using EnrollmentService.Data;
using EnrollmentService.Models;
using MassTransit;
using Users.Contracts.Events;

namespace EnrollmentService.Messaging.Consumers;

public class UserUpsertedConsumer : IConsumer<UserUpsertedV1>
{
    private readonly EnrollmentDbContext _dbContext;

    public UserUpsertedConsumer(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserUpsertedV1> context)
    {
        UserUpsertedV1 message = context.Message;

        KnownUser? user = await _dbContext.KnownUsers.FindAsync(message.UserId);

        if (user is null)
        {
            user = new KnownUser
            {
                UserId = message.UserId,
                Version = message.Version,
                IsActive = true,
                UpdatedAtUtc = message.UpdatedAtUtc
            };

            _dbContext.KnownUsers.Add(user);
        }

        if (message.Version > user.Version)
        {
            user.Version = message.Version;
            user.IsActive = true;
            user.UpdatedAtUtc = message.UpdatedAtUtc;
        }

        await _dbContext.SaveChangesAsync();
    }
}
