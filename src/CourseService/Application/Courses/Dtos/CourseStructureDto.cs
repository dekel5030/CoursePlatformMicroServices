namespace Courses.Application.Courses.Dtos;

public record CourseStructureDto
{
    public required IReadOnlyList<Guid> ModuleIds { get; init; }
    public required IReadOnlyDictionary<Guid, IReadOnlyList<Guid>> ModuleLessonIds { get; init; }
}
