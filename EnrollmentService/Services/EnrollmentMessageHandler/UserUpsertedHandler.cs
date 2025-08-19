using Common.Messaging.EventEnvelope;
using EnrollmentService.Data;
using EnrollmentService.Models;
using Users.Contracts.Events;

namespace EnrollmentService.Services.EnrollmentMessageHandler;

public class UserUpsertedHandler : IEnvelopeHandler<UserUpsertedV1>
{
    private readonly EnrollmentDbContext _dbContext;
    public UserUpsertedHandler(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(EventEnvelope<UserUpsertedV1> envelope, CancellationToken ct = default)
    {
        UserUpsertedV1 message = envelope.Payload;
        KnownUser? user = await _dbContext.KnownUsers
            .FindAsync(new object?[] { message.UserId }, ct);

        if (user is null)
        {
            user = CreateUser(envelope);
            await _dbContext.KnownUsers.AddAsync(user, ct);
        }
        else if (envelope.AggregateVersion > user.AggregateVersion)
        {
            ApplyUserUpdates(user, envelope);
        }
        else
        {
            return;
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    private KnownUser CreateUser(EventEnvelope<UserUpsertedV1> envelope)
    {
        KnownUser user = new KnownUser
        {
            UserId = envelope.Payload.UserId,
        };

        ApplyUserUpdates(user, envelope);

        return user;
    }

    private void ApplyUserUpdates(KnownUser user, EventEnvelope<UserUpsertedV1> envelope)
    {
        user.AggregateVersion = envelope.AggregateVersion;
        user.UpdatedAtUtc = envelope.OccurredAtUtc;
        user.IsActive = envelope.Payload.IsActive;
    }
}