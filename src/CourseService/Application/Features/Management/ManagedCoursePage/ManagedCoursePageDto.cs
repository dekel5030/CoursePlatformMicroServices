using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;

namespace Courses.Application.Features.Management.ManagedCoursePage;

public record ManagedCoursePageDto
{
    public required CourseDto Course { get; init; }
    public required CourseStructureDto Structure { get; init; }

    public required IReadOnlyDictionary<Guid, ManagedModuleDto> Modules { get; init; }
    public required IReadOnlyDictionary<Guid, LessonDto> Lessons { get; init; }
    public required IReadOnlyDictionary<Guid, CategoryDto> Categories { get; init; }
}
