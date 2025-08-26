using Common.Messaging.EventEnvelope;
using Courses.Contracts.Events;

namespace CourseService.Messaging.Publisher;

public interface ICourseEventPublisher
{
    Task PublishCourseUpsertedEvent(EventEnvelope<CourseUpsertedV1> envelope, CancellationToken ct = default);
}