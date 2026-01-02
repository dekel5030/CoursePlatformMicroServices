namespace Courses.Application.Courses.Queries.Dtos;

public record LessonSummaryDto(
    Guid Id,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl
);