using Common.Messaging.EventEnvelope;
using EnrollmentService.Data;
using EnrollmentService.Models;
using MassTransit;
using Users.Contracts.Events;

namespace EnrollmentService.Messaging.Consumers;

public class UserUpsertedConsumer : IConsumer<EventEnvelope<UserUpsertedV1>>
{
    private readonly EnrollmentDbContext _dbContext;

    public UserUpsertedConsumer(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EventEnvelope<UserUpsertedV1>> context)
    {
        EventEnvelope<UserUpsertedV1> envelope = context.Message;
        UserUpsertedV1 message = envelope.Payload;

        KnownUser? user = await _dbContext.KnownUsers.FindAsync(message.UserId);

        if (user is null)
        {
            user = new KnownUser
            {
                UserId = message.UserId,
                IsActive = true,
                UpdatedAtUtc = message.UpdatedAtUtc
            };

            _dbContext.KnownUsers.Add(user);
        }
        else if (envelope.OccurredAtUtc > user.UpdatedAtUtc)
        {
            user.IsActive = true;
            user.UpdatedAtUtc = message.UpdatedAtUtc;
        }

        await _dbContext.SaveChangesAsync();
    }
}
