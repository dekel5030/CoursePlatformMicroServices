using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonSummaryDto(
    Guid ModuleId,
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access
);
