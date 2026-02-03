namespace Courses.Application.Modules.Dtos;

public sealed record ModuleSummaryWithAnalyticsDto(
    ModuleSummaryDto Summary,
    ModuleSummaryAnalyticsDto Analytics
);
