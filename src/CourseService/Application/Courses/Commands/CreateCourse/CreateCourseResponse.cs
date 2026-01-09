using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Courses.Commands.CreateCourse;

public record CreateCourseResponse(CourseId CourseId, string Title);