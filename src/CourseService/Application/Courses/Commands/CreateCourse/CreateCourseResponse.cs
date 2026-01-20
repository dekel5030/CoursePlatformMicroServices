using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Courses.Commands.CreateCourse;

public sealed record CreateCourseResponse(CourseId Id, Title Title);
