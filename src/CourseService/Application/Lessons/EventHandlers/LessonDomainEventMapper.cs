using CoursePlatform.Contracts.CourseService;
using Courses.Domain.Lessons;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.EventHandlers;

internal sealed class LessonDomainEventMapper :
    IDomainEventHandler<LessonCreatedDomainEvent>,
    IDomainEventHandler<LessonMetadataChangedDomainEvent>,
    IDomainEventHandler<LessonMediaChangedDomainEvent>,
    IDomainEventHandler<LessonAccessChangedDomainEvent>,
    IDomainEventHandler<LessonIndexChangedDomainEvent>,
    IDomainEventHandler<LessonTranscriptChangedDomainEvent>,
    IDomainEventHandler<LessonDeletedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public LessonDomainEventMapper(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task HandleAsync(LessonCreatedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonCreatedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value,
            message.Slug.Value,
            message.Title.Value,
            message.Description.Value,
            message.Access.ToString(),
            message.Duration,
            message.Index,
            message.VideoUrl?.Path,
            message.ThumbnailUrl?.Path,
            message.TranscriptUrl?.Path), cancellationToken);
    }

    public Task HandleAsync(LessonMetadataChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonMetadataChangedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value,
            message.Title.Value,
            message.Description.Value,
            message.Slug.Value), cancellationToken);
    }

    public Task HandleAsync(LessonMediaChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonMediaChangedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value,
            message.VideoUrl?.Path,
            message.ThumbnailUrl?.Path,
            message.Duration), cancellationToken);
    }

    public Task HandleAsync(LessonAccessChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonAccessChangedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value,
            message.NewAccess.ToString()), cancellationToken);
    }

    public Task HandleAsync(LessonIndexChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonIndexChangedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value,
            message.NewIndex), cancellationToken);
    }

    public Task HandleAsync(LessonTranscriptChangedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonTranscriptChangedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value,
            message.TranscriptUrl?.Path), cancellationToken);
    }

    public Task HandleAsync(LessonDeletedDomainEvent message, CancellationToken cancellationToken = default)
    {
        return _eventBus.PublishAsync(new LessonDeletedIntegrationEvent(
            message.Id.Value,
            message.ModuleId.Value,
            message.CourseId.Value), cancellationToken);
    }
}