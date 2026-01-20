using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Actions;

public record CoursePolicyContext(
    CourseId CourseId,
    UserId InstructorId,
    CourseStatus Status,
    bool IsDeleted,
    int LessonCount
);
