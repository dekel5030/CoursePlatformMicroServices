using Courses.Application.Features.Dtos;
using Courses.Application.ReadModels;

namespace Courses.Application.Features.Shared.Mappers;

public static class CourseAnalyticsDtoMapper
{
    public static CourseSummaryAnalyticsDto ToSummaryAnalytics(CourseAnalytics? source)
    {
        return new CourseSummaryAnalyticsDto(
            LessonsCount: source?.TotalLessonsCount ?? 0,
            Duration: source?.TotalCourseDuration ?? TimeSpan.Zero,
            EnrollmentCount: source?.EnrollmentsCount ?? 0,
            AverageRating: source?.AverageRating ?? 0,
            ReviewsCount: source?.ReviewsCount ?? 0,
            CourseViews: source?.ViewCount ?? 0);
    }

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
