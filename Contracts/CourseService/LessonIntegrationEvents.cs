namespace CoursePlatform.Contracts.CourseService;

public sealed record LessonCreatedIntegrationEvent(
    Guid LessonId,
    Guid ModuleId,
    Guid CourseId,
    string Slug,
    string Title,
    string Description,
    string Access,
    TimeSpan Duration,
    int Index,
    string? VideoUrl,
    string? ThumbnailUrl,
    string? TranscriptUrl);

public sealed record LessonMetadataChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string Title,
    string Description,
    string Slug);

public sealed record LessonMediaChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string? VideoUrl,
    string? ThumbnailUrl,
    TimeSpan Duration);

public sealed record LessonAccessChangedIntegrationEvent(
    Guid LessonId,
    Guid ModuleId,
    Guid CourseId,
    string NewAccess);

public sealed record LessonIndexChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    int NewIndex);

public sealed record LessonTranscriptChangedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId,
    string? TranscriptUrl);

public sealed record LessonDeletedIntegrationEvent(
    Guid Id,
    Guid ModuleId,
    Guid CourseId);