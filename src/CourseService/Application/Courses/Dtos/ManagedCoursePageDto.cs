using Courses.Application.Categories.Dtos;
using Courses.Application.Modules.Dtos;

namespace Courses.Application.Courses.Dtos;

/// <summary>
/// Response for the instructor's course detail/editor view. Excludes Analytics and Instructors (instructor is the current user).
/// </summary>
public record ManagedCoursePageDto
{
    public required CourseDto Course { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, ModuleWithAnalyticsDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, LessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}
