using Courses.Application.Lessons.Dtos;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Features.Management.ManagedCoursePage.Dtos;

public record ManagedCoursePageLessonDto
{
    public required LessonDto Lesson { get; set; }
    public required ManagedLessonLinks Links { get; init; }
}
