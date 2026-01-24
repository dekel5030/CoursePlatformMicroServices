using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Services.Actions.Courses;

public record CourseState(
    CourseId CourseId,
    UserId InstructorId,
    CourseStatus Status,
    int LessonCount
);
