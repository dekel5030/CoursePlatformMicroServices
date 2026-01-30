using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Lessons.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

/// <summary>
/// Projector for LessonReadModel - maintains lesson projections.
/// Uses ONLY Integration Events (no WriteDbContext dependency).
/// </summary>
internal sealed class LessonProjector :
    IEventConsumer<LessonCreatedIntegrationEvent>,
    IEventConsumer<LessonMetadataChangedIntegrationEvent>,
    IEventConsumer<LessonMediaChangedIntegrationEvent>,
    IEventConsumer<LessonAccessChangedIntegrationEvent>,
    IEventConsumer<LessonIndexChangedIntegrationEvent>,
    IEventConsumer<LessonTranscriptChangedIntegrationEvent>,
    IEventConsumer<LessonDeletedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public LessonProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var lesson = new LessonReadModel
        {
            Id = message.LessonId,
            ModuleId = message.ModuleId,
            CourseId = message.CourseId,
            Title = message.Title,
            Description = message.Description,
            Slug = message.Slug,
            Index = message.Index,
            Access = Enum.Parse<LessonAccess>(message.Access),
            VideoUrl = message.VideoUrl,
            ThumbnailUrl = message.ThumbnailUrl,
            TranscriptUrl = message.TranscriptUrl,
            Duration = message.Duration,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _readDbContext.Lessons.Add(lesson);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        LessonMetadataChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateLessonAsync(
            message.Id,
            lesson =>
            {
                lesson.Title = message.Title;
                lesson.Description = message.Description;
                lesson.Slug = message.Slug;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateLessonAsync(
            message.Id,
            lesson =>
            {
                lesson.VideoUrl = message.VideoUrl;
                lesson.ThumbnailUrl = message.ThumbnailUrl;
                lesson.Duration = message.Duration;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        LessonAccessChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateLessonAsync(
            message.LessonId,
            lesson => lesson.Access = Enum.Parse<LessonAccess>(message.NewAccess),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonIndexChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateLessonAsync(
            message.Id,
            lesson => lesson.Index = message.NewIndex,
            cancellationToken);
    }

    public Task HandleAsync(
        LessonTranscriptChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateLessonAsync(
            message.Id,
            lesson => lesson.TranscriptUrl = message.TranscriptUrl,
            cancellationToken);
    }

    public async Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        LessonReadModel? lesson = await _readDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == message.Id, cancellationToken);

        if (lesson is not null)
        {
            _readDbContext.Lessons.Remove(lesson);
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Helpers

    private async Task UpdateLessonAsync(
        Guid lessonId,
        Action<LessonReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        LessonReadModel? lesson = await _readDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            return;
        }

        updateAction(lesson);
        lesson.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _readDbContext.SaveChangesAsync(cancellationToken);
    }
}
