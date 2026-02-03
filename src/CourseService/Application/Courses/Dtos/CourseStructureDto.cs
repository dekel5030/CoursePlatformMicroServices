namespace Courses.Application.Courses.Dtos;

public record CourseStructureDto
{
    // Ordered module IDs (display order)
    public required IReadOnlyList<Guid> ModuleIds { get; init; }
    // Per module: ordered lesson IDs. Key = moduleId
    public required IReadOnlyDictionary<Guid, IReadOnlyList<Guid>> ModuleLessonIds { get; init; }
}
