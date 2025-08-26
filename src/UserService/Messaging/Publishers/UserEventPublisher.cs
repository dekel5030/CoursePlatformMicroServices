using Common.Messaging;
using Common.Messaging.EventEnvelope;
using MassTransit;
using Users.Contracts.Events;

namespace UserService.Messaging.Publishers;

public class UserEventPublisher : IUserEventPublisher
{
    private readonly IPublishEndpoint _publish;

    public UserEventPublisher(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public Task PublishUserUpsertedAsync(EventEnvelope<UserUpsertedV1> envelope, CancellationToken ct = default)
    {
        return _publish.Publish(envelope, ctx =>
        {
            ctx.Headers.Set(HeaderNames.CorrelationId, envelope.CorrelationId);
        }, ct);
    }
}
