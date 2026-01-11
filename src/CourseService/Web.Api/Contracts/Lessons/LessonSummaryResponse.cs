namespace Courses.Api.Contracts.Lessons;

public record LessonSummaryResponse(
    Guid LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl);
