namespace Courses.Application.Pages.Dtos;

public record CourseStructureDto
{
    public required IReadOnlyList<Guid> ModuleIds { get; init; }
    public required IReadOnlyDictionary<Guid, IReadOnlyList<Guid>> ModuleLessonIds { get; init; }
}
