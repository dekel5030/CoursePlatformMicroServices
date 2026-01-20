using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Actions;

public record LessonPolicyContext(
    LessonId LessonId,
    LessonStatus Status,
    LessonAccess Access,
    bool IsDraft
);
