using CoursePlatform.Contracts.CourseEvents;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Events;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.EventHandlers;

internal sealed class CourseCreatedDomainEventHandler : IDomainEventHandler<CourseCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;
    private readonly TimeProvider _timeProvider;

    public CourseCreatedDomainEventHandler(IEventBus eventBus, TimeProvider timeProvider)
    {
        _eventBus = eventBus;
        _timeProvider = timeProvider;
    }

    public Task HandleAsync(
        CourseCreatedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        ICourseSnapshot course = message.Course;

        var integrationEvent = new CourseCreatedIntegrationEvent
        {
            CourseId = course.Id.Value,
            InstructorId = course.InstructorId.Value,
            Title = course.Title.Value,
            Slug = course.Slug.Value,
            PriceAmount = course.Price.Amount,
            PriceCurrency = course.Price.Currency,
            Status = course.Status.ToString(),
            CreatedAt = _timeProvider.GetUtcNow(),
        };

        return _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
