using Common.Messaging;
using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;
using Courses.Contracts.Routing;
using CourseService.Dtos.CourseEvents;
using MassTransit;

namespace CourseService.Messaging.Publisher;

public class CourseEventPublisher : ICourseEventPublisher
{
    private readonly IPublishEndpoint _publish;
    private readonly string _source = "CourseService";

    public CourseEventPublisher(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public async Task PublishCourseRemovedEvent(int courseId, string correlationId, CancellationToken ct = default)
    {
        var message = new CourseRemovedV1(CourseId: courseId);

        var envelope = CreateEnvelope(correlationId, message, CourseRemovedV1.Version, CourseRemovedV1.EventType);

        await _publish.Publish(envelope, ctx =>
        {
            ctx.SetRoutingKey(RoutingKeys.Removed(CourseRemovedV1.Version));
            ctx.Headers.Set(HeaderNames.CorrelationId, correlationId);
        }, ct);
    }

    public Task PublishCourseUpsertedEvent(CourseUpsertedEventDto course, string correlationId, CancellationToken ct = default)
    {
        var message = new CourseUpsertedV1
        (
            CourseId: course.CourseId,
            IsPublished: course.IsPublished
        );

        var envelope = CreateEnvelope(correlationId, message, CourseUpsertedV1.Version, CourseUpsertedV1.EventType);

        return _publish.Publish(envelope, ctx =>
        {
            ctx.SetRoutingKey(RoutingKeys.Upserted(CourseUpsertedV1.Version));
            ctx.Headers.Set(HeaderNames.CorrelationId, correlationId);
        }, ct);
    }

    private EventEnvelope<T> CreateEnvelope<T>(string correlationId, T payload, int version, string eventType)
    {
        return new EventEnvelope<T>
        (
            EventId: Guid.NewGuid(),
            CorrelationId: correlationId,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            Source: _source,
            EventType: eventType,
            Version: version,
            Payload: payload
        );
    }
}
