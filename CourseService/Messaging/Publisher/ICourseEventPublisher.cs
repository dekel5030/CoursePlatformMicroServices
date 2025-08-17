using CourseService.Dtos.CourseEvents;

namespace CourseService.Messaging.Publisher;

public interface ICourseEventPublisher
{
    Task PublishCourseUpsertedEvent(CourseUpsertedEventDto course, string correlationId, CancellationToken ct = default);
    Task PublishCourseRemovedEvent(int courseId, string correlationId, CancellationToken ct = default);
}