using Courses.Domain.Courses.Primitives;

namespace Courses.Application.ReadModels;

public interface ICourseAnalyticsProjection
{
    Task RecalculateAsync(CourseId courseId, CancellationToken cancellationToken = default);
}
