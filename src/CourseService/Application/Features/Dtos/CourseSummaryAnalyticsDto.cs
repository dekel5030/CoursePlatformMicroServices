namespace Courses.Application.Features.Dtos;

public sealed record CourseSummaryAnalyticsDto(
    int LessonsCount,
    TimeSpan Duration,
    int EnrollmentCount,
    double AverageRating,
    int ReviewsCount,
    int CourseViews
);
