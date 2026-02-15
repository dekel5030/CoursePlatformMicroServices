using Courses.Application.Features.CourseCatalog;
using Courses.Application.Features.CoursePage;
using Courses.Application.ReadModels;

namespace Courses.Application.Features.Shared.Mappers;

public static class CourseAnalyticsDtoMapper
{
    public static CourseAnalyticsDto ToCourseAnalytics(CourseAnalytics? source)
    {
        return source != null
            ? new CourseAnalyticsDto(
                source.EnrollmentsCount,
                source.TotalLessonsCount,
                source.TotalCourseDuration,
                source.AverageRating,
                source.ReviewsCount,
                source.ViewCount)
            : new CourseAnalyticsDto(0, 0, TimeSpan.Zero, 0, 0, 0);
    }
}
