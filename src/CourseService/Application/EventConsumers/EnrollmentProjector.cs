using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Kernel.EventBus;

namespace Courses.Application.EventConsumers;

internal sealed class EnrollmentProjector :
    IEventConsumer<EnrollmentCreatedIntegrationEvent>,
    IEventConsumer<EnrollmentStatusChangedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public EnrollmentProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public Task HandleAsync(
        EnrollmentCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatsAsync(
            message.CourseId,
            stats => stats.IncrementEnrollments(),
            cancellationToken);
    }

    public async Task HandleAsync(
        EnrollmentStatusChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
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
