using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public record ManagedCoursePageCourseDto
{
    public required CourseDto Course { get; init; }
    public required ManagedCourseLinks Links { get; init; }
}
