using Courses.Application.Abstractions.Analytics;
using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Courses.Infrastructure.Analytics;

internal sealed class CourseViewAggregationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CourseViewAggregationBackgroundService> _logger;
    private readonly TimeSpan _aggregationInterval = TimeSpan.FromMinutes(5);

    public CourseViewAggregationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CourseViewAggregationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Course View Aggregation Background Service started");

        await AggregateViewsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_aggregationInterval, stoppingToken);

                await AggregateViewsAsync(stoppingToken);
            }
            catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(ex, "Course View Aggregation Background Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while aggregating course views");
            }
        }
    }

    private async Task AggregateViewsAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();

        ICourseViewTrackingService viewTrackingService = scope.ServiceProvider.GetRequiredService<ICourseViewTrackingService>();
        IWriteDbContext writeDbContext = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        Dictionary<CourseId, long> pendingViews = await viewTrackingService.GetAllPendingViewsAsync(cancellationToken);

        if (pendingViews.Count == 0)
        {
            _logger.LogDebug("No pending views to aggregate");
            return;
        }

        _logger.LogInformation("Aggregating views for {Count} courses", pendingViews.Count);

        var courseIds = pendingViews.Keys.ToList();
        List<Domain.Courses.Course> courses = await writeDbContext.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        var courseLookup = courses.ToDictionary(c => c.Id);

        foreach ((CourseId courseId, long viewCount) in pendingViews)
        {
            if (!courseLookup.TryGetValue(courseId, out Domain.Courses.Course? course))
            {
                _logger.LogWarning("Course {CourseId} not found for view aggregation", courseId);
                await viewTrackingService.ClearPendingViewsAsync(courseId, cancellationToken);
                continue;
            }

            course.IncrementViewCount(viewCount);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (CourseId courseId in pendingViews.Keys)
        {
            await viewTrackingService.ClearPendingViewsAsync(courseId, cancellationToken);
        }

        _logger.LogInformation("Successfully aggregated views for {Count} courses", pendingViews.Count);
    }
}
