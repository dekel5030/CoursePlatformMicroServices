namespace Courses.Application.Courses.Dtos;

public sealed record CourseAnalyticsDto(
    int EnrollmentCount,
    int LessonsCount,
    TimeSpan TotalDuration,
    double AverageRating,
    int ReviewsCount,
    int ViewCount
);
