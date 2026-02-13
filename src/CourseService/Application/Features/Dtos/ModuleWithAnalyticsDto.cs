using Courses.Application.Modules.Dtos;

namespace Courses.Application.Features.Dtos;

public sealed record ModuleWithAnalyticsDto(
    ModuleDto Module,
    ModuleAnalyticsDto Analytics
);
