using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Kernel.EventBus;

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
            stats => stats.IncrementModules(),
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.DecrementModules(),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.AddLesson(message.Id, message.Duration),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.UpdateLessonDuration(message.Id, message.Duration),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.RemoveLesson(message.Id),
            cancellationToken);
    }

    private async Task UpdateStatsAsync(
        Guid courseId,
        Action<CourseStatsReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        CourseStatsReadModel? stats = await _readDbContext.CourseStats
            .FindAsync([courseId], cancellationToken);

        if (stats is not null)
        {
            updateAction(stats);
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
