using Common.Messaging.EventEnvelope;
using CourseService.Services.Handlers;
using Enrollments.Contracts.Events;
using MassTransit;

namespace CourseService.Messaging.Consumer;

public class EnrollmentUpsertedConsumer : IConsumer<EventEnvelope<EnrollmentUpsertedV1>>
{
    private readonly IEnvelopeHandler<EnrollmentUpsertedV1> _handler;

    public EnrollmentUpsertedConsumer(IEnvelopeHandler<EnrollmentUpsertedV1> handler)
    {
        _handler = handler;
    }

    public Task Consume(ConsumeContext<EventEnvelope<EnrollmentUpsertedV1>> context)
    {
        return _handler.HandleAsync(context.Message, context.CancellationToken);
    }
}