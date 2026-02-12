using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Pages.Dtos;

namespace Courses.Application.Pages.CoursePage;

public record CoursePageDto
{
    public required CourseDto Course { get; init; }
    public required CourseAnalyticsDto Analytics { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, ModuleWithAnalyticsDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, LessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, UserDto> Instructors { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}

