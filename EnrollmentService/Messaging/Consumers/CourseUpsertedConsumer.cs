using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using EnrollmentService.Services.EnrollmentMessageHandler;
using MassTransit;

namespace EnrollmentService.Messaging.Consumers;

public class CourseUpsertedConsumer : IConsumer<EventEnvelope<CourseUpsertedV1>>
{
    private readonly IEnvelopeHandler<CourseUpsertedV1> _handler;

    public CourseUpsertedConsumer(IEnvelopeHandler<CourseUpsertedV1> handler)
    {
        _handler = handler;
    }
    
    public Task Consume(ConsumeContext<EventEnvelope<CourseUpsertedV1>> context)
    {
        return _handler.HandleAsync(context.Message, context.CancellationToken);
    }
}
