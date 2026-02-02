using Courses.Application.Categories.Dtos;

namespace Courses.Application.Courses.Dtos;

public record CoursePageDto
{
    public required CourseDto Course { get; init; }

    public required IReadOnlyDictionary<Guid, ModuleDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, LessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, UserDto> Instructors { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}
