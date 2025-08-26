using Common.Messaging;
using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using MassTransit;

namespace CourseService.Messaging.Publisher;

public class CourseEventPublisher : ICourseEventPublisher
{
    private readonly IPublishEndpoint _publish;

    public CourseEventPublisher(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public Task PublishCourseUpsertedEvent(EventEnvelope<CourseUpsertedV1> envelope, CancellationToken ct = default)
    {
        return _publish.Publish(envelope, ctx =>
        {
            ctx.Headers.Set(HeaderNames.CorrelationId, envelope.CorrelationId);
        }, ct);
    }
}
