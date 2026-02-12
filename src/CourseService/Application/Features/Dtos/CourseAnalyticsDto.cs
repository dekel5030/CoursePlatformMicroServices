namespace Courses.Application.Features.Dtos;

public sealed record CourseAnalyticsDto(
    int EnrollmentCount,
    int LessonsCount,
    TimeSpan TotalDuration,
    double AverageRating,
    int ReviewsCount,
    int ViewCount
);
