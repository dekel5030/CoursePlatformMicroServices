using Courses.Application.Categories.Dtos;
using Courses.Application.Modules.Dtos;

namespace Courses.Application.Courses.Dtos;

public record CoursePageDto
{
    public required CourseWithAnalyticsDto Course { get; init; }

    public required IReadOnlyDictionary<Guid, ModuleWithAnalyticsDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, LessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, UserDto> Instructors { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}
