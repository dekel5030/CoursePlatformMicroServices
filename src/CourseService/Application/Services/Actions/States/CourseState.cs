using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Services.Actions.States;

public record CourseState(
    CourseId Id,
    UserId InstructorId,
    CourseStatus Status,
    int LessonCount
);
