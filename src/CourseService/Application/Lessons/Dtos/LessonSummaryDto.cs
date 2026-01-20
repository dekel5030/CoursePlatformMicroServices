using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonSummaryDto(
    CourseId CourseId,
    LessonId LessonId,
    Title Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonStatus Status,
    LessonAccess Access
);
