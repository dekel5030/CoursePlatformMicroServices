using Courses.Domain.Courses;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Ratings;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.ReadModels;

internal sealed class CourseAnalyticsProjectionHandlers :
    IDomainEventHandler<CourseCreatedDomainEvent>,
    IDomainEventHandler<LessonCreatedDomainEvent>,
    IDomainEventHandler<LessonDeletedDomainEvent>,
    IDomainEventHandler<LessonMediaChangedDomainEvent>,
    IDomainEventHandler<LessonMovedDomainEvent>,
    IDomainEventHandler<ModuleCreatedDomainEvent>,
    IDomainEventHandler<ModuleDeletedDomainEvent>,
    IDomainEventHandler<EnrollmentCreatedDomainEvent>,
    IDomainEventHandler<CourseRatingCreatedDomainEvent>,
    IDomainEventHandler<CourseRatingUpdatedDomainEvent>
{
    private readonly ICourseAnalyticsProjection _projection;

    public CourseAnalyticsProjectionHandlers(ICourseAnalyticsProjection projection)
    {
        _projection = projection;
    }

    public Task HandleAsync(CourseCreatedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(LessonCreatedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(LessonDeletedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(LessonMediaChangedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(LessonMovedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(ModuleCreatedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(ModuleDeletedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(EnrollmentCreatedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(CourseRatingCreatedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);

    public Task HandleAsync(CourseRatingUpdatedDomainEvent message, CancellationToken cancellationToken = default)
        => _projection.RecalculateAsync(message.CourseId, cancellationToken);
}
