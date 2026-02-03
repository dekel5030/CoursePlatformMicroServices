using Courses.Application.Courses.Dtos;

namespace Courses.Application.Modules.Dtos;

/// <summary>
/// Module with analytics and structure (lesson order). Used by GetModules API.
/// </summary>
public sealed record ModuleWithAnalyticsAndStructureDto(
    ModuleDto Module,
    ModuleAnalyticsDto Analytics,
    IReadOnlyList<Guid> LessonIds
);
