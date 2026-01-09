namespace Courses.Application.Lessons.Queries.Dtos;

public record LessonDetailsDto(
    Guid CourseId,
    Guid LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl,
    string? VideoUrl
);