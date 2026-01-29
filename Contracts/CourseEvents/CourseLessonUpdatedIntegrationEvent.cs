namespace CoursePlatform.Contracts.CourseEvents;

public record CourseLessonUpdatedIntegrationEvent
{
    public required Guid CourseId { get; init; }
    public required Guid ModuleId { get; init; }
    public required Guid LessonId { get; init; }
    public required string Title { get; init; }
    public required int Index { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string? ThumbnailUrl { get; init; }
    public required string Access { get; init; }
}
