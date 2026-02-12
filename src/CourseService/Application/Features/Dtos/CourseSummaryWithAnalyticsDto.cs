namespace Courses.Application.Features.Dtos;

public sealed record CourseSummaryWithAnalyticsDto(
    CourseSummaryDto Course,
    CourseSummaryAnalyticsDto Analytics
);
