using CoursePlatform.Contracts.CourseEvents;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Events;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.EventHandlers;

internal sealed class CourseUpdatedDomainEventHandler : IDomainEventHandler<CourseUpdatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CourseUpdatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(
        CourseUpdatedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        ICourseSnapshot course = message.Course;

        var integrationEvent = new CourseUpdatedEvent(
            CourseId: course.Id.Value.ToString(),
            Title: course.Title.Value,
            IsPublished: course.Status.ToString().Equals("Published", StringComparison.OrdinalIgnoreCase)
        );

        return _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
