namespace Courses.Application.Lessons.Queries.Dtos;

public record LessonSummaryDto(
    Guid CourseId,
    Guid LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl
);