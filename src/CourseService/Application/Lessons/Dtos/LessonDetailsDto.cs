using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Lessons.Dtos;

public record LessonDetailsDto(
    Guid ModuleId,
    Guid LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    LessonAccess Access,
    string? VideoUrl
);
