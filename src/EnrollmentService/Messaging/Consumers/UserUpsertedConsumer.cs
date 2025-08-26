using Common.Messaging.EventEnvelope;
using EnrollmentService.Services.EnrollmentMessageHandler;
using MassTransit;
using Users.Contracts.Events;

namespace EnrollmentService.Messaging.Consumers;

public class UserUpsertedConsumer : IConsumer<EventEnvelope<UserUpsertedV1>>
{
    private readonly IEnvelopeHandler<UserUpsertedV1> _handler;

    public UserUpsertedConsumer(IEnvelopeHandler<UserUpsertedV1> handler)
    {
        _handler = handler;
    }

    public Task Consume(ConsumeContext<EventEnvelope<UserUpsertedV1>> context)
    {
        return _handler.HandleAsync(context.Message, context.CancellationToken);
    }
}
