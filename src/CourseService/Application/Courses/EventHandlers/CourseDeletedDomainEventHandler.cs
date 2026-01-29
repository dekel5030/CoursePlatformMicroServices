using CoursePlatform.Contracts.CourseEvents;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Events;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.EventHandlers;

internal sealed class CourseDeletedDomainEventHandler : IDomainEventHandler<CourseDeletedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CourseDeletedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(
        CourseDeletedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        ICourseSnapshot course = message.Course;

        var integrationEvent = new CourseDeletedEvent(
            CourseId: course.Id.Value.ToString());

        return _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
