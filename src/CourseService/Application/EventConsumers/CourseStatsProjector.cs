using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

internal sealed class CourseStatsProjector :
    IEventConsumer<CourseCreatedIntegrationEvent>,
    IEventConsumer<ModuleCreatedIntegrationEvent>,
    IEventConsumer<ModuleDeletedIntegrationEvent>,
    IEventConsumer<LessonCreatedIntegrationEvent>,
    IEventConsumer<LessonMediaChangedIntegrationEvent>,
    IEventConsumer<LessonDeletedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public CourseStatsProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        CourseCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var stats = new CourseStatsReadModel
        {
            Id = Guid.NewGuid(),
            CourseId = message.CourseId,
            TotalDuration = TimeSpan.Zero,
            LessonsCount = 0,
            ModulesCount = 0,
            EnrollmentCount = 0
        };

        _readDbContext.CourseStats.Add(stats);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        ModuleCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.ModulesCount++,
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.ModulesCount--,
            cancellationToken);
    }

    public Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats =>
            {
                stats.LessonsCount++;
                stats.TotalDuration += message.Duration;
            },
            cancellationToken);
    }

    public async Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        CourseStructureReadModel? structure = await _readDbContext.CourseStructures
            .FirstOrDefaultAsync(s => s.CourseId == message.CourseId, cancellationToken);

        if (structure is null)
        {
            return;
        }

        StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
        StructureLessonReadModel? lesson = module?.Lessons.Find(l => l.Id == message.Id);

        if (lesson is null)
        {
            return;
        }

        TimeSpan oldDuration = lesson.Duration;
        TimeSpan durationDiff = message.Duration - oldDuration;

        await UpdateStatsAsync(
            message.CourseId,
            stats => stats.TotalDuration += durationDiff,
            cancellationToken);
    }

    public async Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        CourseStructureReadModel? structure = await _readDbContext.CourseStructures
            .FirstOrDefaultAsync(s => s.CourseId == message.CourseId, cancellationToken);

        if (structure is null)
        {
            return;
        }

        StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
        StructureLessonReadModel? lesson = module?.Lessons.Find(l => l.Id == message.Id);

        if (lesson is null)
        {
            return;
        }

        TimeSpan lessonDuration = lesson.Duration;

        await UpdateStatsAsync(
            message.CourseId,
            stats =>
            {
                stats.LessonsCount--;
                stats.TotalDuration -= lessonDuration;
            },
            cancellationToken);
    }

    private async Task UpdateStatsAsync(
        Guid courseId,
        Action<CourseStatsReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        CourseStatsReadModel? stats = await _readDbContext.CourseStats
            .FirstOrDefaultAsync(s => s.CourseId == courseId, cancellationToken);

        if (stats is not null)
        {
            updateAction(stats);
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
