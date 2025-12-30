using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Courses.Queries.Dtos;

public record LessonReadDto(
    Guid Id,
    string Title,
    string Description,
    LessonAccess Access,
    LessonStatus Status,
    int Index,
    string? ThumbnailImageUrl,
    string? VideoUrl,
    TimeSpan? Duration);
