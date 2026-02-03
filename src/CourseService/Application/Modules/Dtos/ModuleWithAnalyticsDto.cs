using Courses.Application.Courses.Dtos;

namespace Courses.Application.Modules.Dtos;

public sealed record ModuleWithAnalyticsDto(
    ModuleDto Module,
    ModuleAnalyticsDto Analytics
);
