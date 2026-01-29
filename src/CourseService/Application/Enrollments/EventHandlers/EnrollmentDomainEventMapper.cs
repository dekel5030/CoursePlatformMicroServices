using CoursePlatform.Contracts.CourseService;
using Courses.Domain.Enrollments;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.EventHandlers;

internal sealed class EnrollmentDomainEventMapper :
    IDomainEventHandler<EnrollmentCreatedDomainEvent>,
    IDomainEventHandler<LessonCompletedDomainEvent>,
    IDomainEventHandler<EnrollmentStatusChangedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public EnrollmentDomainEventMapper(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(EnrollmentCreatedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new EnrollmentCreatedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value,
            message.StudentId.Value,
            message.EnrolledAt), cancellationToken);
    }

    public Task HandleAsync(LessonCompletedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonCompletedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value,
            message.StudentId.Value,
            message.LessonId.Value,
            message.CourseFullyCompleted), cancellationToken);
    }

    public Task HandleAsync(EnrollmentStatusChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new EnrollmentStatusChangedIntegrationEvent(
            message.Id.Value,
            message.CourseId.Value,
            message.NewStatus.ToString()), cancellationToken);
    }
}