using Courses.Application.Courses.Dtos;

namespace Courses.Application.Features.Dtos;

public sealed record CourseWithAnalyticsDto(
    CourseDto Course,
    CourseAnalyticsDto Analytics
);
