using CoursePlatform.Contracts.CourseEvents;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Events;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.EventHandlers;

internal sealed class CoursePublishedDomainEventHandler : IDomainEventHandler<CoursePublishedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public CoursePublishedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(
        CoursePublishedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        ICourseSnapshot course = message.Course;

        var integrationEvent = new CourseUpdatedEvent(
            CourseId: course.Id.Value.ToString(),
            Title: course.Title.Value,
            IsPublished: true
        );

        return _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
