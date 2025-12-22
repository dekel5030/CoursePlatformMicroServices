using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record LessonReadDto(
    LessonId Id,
    string Title,
    string? Description,
    string? VideoUrl,
    string? ThumbnailImage,
    bool IsPreview,
    int Order,
    TimeSpan? Duration);
