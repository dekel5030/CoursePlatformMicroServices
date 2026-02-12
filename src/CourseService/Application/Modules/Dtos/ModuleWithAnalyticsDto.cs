namespace Courses.Application.Modules.Dtos;

public sealed record ModuleWithAnalyticsDto(
    ModuleDto Module,
    ModuleAnalyticsDto Analytics
);
