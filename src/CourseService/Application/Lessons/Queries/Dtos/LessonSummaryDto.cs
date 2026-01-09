using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Lessons.Queries.Dtos;

public record LessonSummaryDto(
    CourseId CourseId,
    LessonId LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl
);