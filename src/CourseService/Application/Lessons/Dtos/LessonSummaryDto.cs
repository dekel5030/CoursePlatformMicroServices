using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonSummaryDto(
    ModuleId ModuleId,
    LessonId LessonId,
    Title Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access
);
