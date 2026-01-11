namespace Courses.Api.Contracts.Lessons;

public record LessonDetailsResponse(
    Guid CourseId,
    Guid LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    bool IsPreview,
    string? ThumbnailUrl,
    string? VideoUrl);
