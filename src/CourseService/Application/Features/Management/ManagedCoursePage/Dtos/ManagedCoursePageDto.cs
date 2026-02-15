using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Dtos;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public record ManagedCoursePageDto
{
    public required ManagedCoursePageCourseDto Course { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, ManagedCoursePageModuleDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, ManagedCoursePageLessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}
