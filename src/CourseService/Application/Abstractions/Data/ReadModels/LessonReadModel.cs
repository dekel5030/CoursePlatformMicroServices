using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

/// <summary>
/// Core Read Model for Lesson aggregate.
/// Contains all lesson metadata with ModuleId & CourseId FKs for composition.
/// Self-contained - no need to load full aggregates for read operations.
/// </summary>
public sealed class LessonReadModel
{
    /// <summary>
    /// Primary Key - LessonId (Point Lookup)
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign Key to ModuleReadModel (for composition)
    /// </summary>
    public Guid ModuleId { get; set; }

    /// <summary>
    /// Foreign Key to CourseReadModel (for direct course-lesson queries)
    /// </summary>
    public Guid CourseId { get; set; }

    // Core Metadata
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public int Index { get; set; }

    // Access Control
    public LessonAccess Access { get; set; }

    // Media
    public string? VideoUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TranscriptUrl { get; set; }
    public TimeSpan Duration { get; set; }

    // Timestamps
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
