namespace Courses.Application.Courses.Dtos;

public sealed record CourseSummaryWithAnalyticsDto(
    CourseSummaryDto Course,
    CourseSummaryAnalyticsDto Analytics
);
