using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.ReadModels;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.CourseViews;
using Kernel.EventBus;
using Microsoft.Extensions.Logging;

namespace Courses.Application.EventConsumers;

internal sealed class CourseViewedEventConsumer : IEventConsumer<CourseViewedIntegrationEvent>
{
    private readonly IWriteDbContext _dbContext;
    private readonly ICourseAnalyticsProjection _projection;
    private readonly ILogger<CourseViewedEventConsumer> _logger;

    public CourseViewedEventConsumer(
        IWriteDbContext dbContext,
        ICourseAnalyticsProjection projection,
        ILogger<CourseViewedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _projection = projection;
        _logger = logger;
    }

    public async Task HandleAsync(
        CourseViewedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(message.CourseId);
        UserId? userId = message.UserId.HasValue ? new UserId(message.UserId.Value) : null;

        var courseView = CourseView.Create(courseId, userId, message.ViewedAt);
        _dbContext.CourseViews.Add(courseView);
        _logger.LogDebug("Recorded view for course {CourseId}, user {UserId}", message.CourseId, message.UserId);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _projection.RecalculateAsync(courseId, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
