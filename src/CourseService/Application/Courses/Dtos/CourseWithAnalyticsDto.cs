namespace Courses.Application.Courses.Dtos;

public sealed record CourseWithAnalyticsDto(
    CourseDto Course,
    CourseAnalyticsDto Analytics
);
