using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Analytics;

public interface ICourseViewTrackingService
{
    Task TrackViewAsync(CourseId courseId, CancellationToken cancellationToken = default);

    Task<long> GetPendingViewsAsync(CourseId courseId, CancellationToken cancellationToken = default);

    Task<Dictionary<CourseId, long>> GetAllPendingViewsAsync(CancellationToken cancellationToken = default);

    Task ClearPendingViewsAsync(CourseId courseId, CancellationToken cancellationToken = default);
}
