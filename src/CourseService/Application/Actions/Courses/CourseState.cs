using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Actions.Courses;

public record CourseState(
    CourseId CourseId,
    UserId InstructorId,
    CourseStatus Status,
    int LessonCount
);
