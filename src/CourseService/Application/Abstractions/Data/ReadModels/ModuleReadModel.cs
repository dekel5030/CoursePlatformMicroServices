namespace Courses.Application.Abstractions.Data.ReadModels;

/// <summary>
/// Core Read Model for Module aggregate.
/// Contains module metadata with CourseId FK for composition.
/// Can be queried independently and composed with LessonReadModel.
/// </summary>
public sealed class ModuleReadModel
{
    /// <summary>
    /// Primary Key - ModuleId (Point Lookup)
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign Key to CourseReadModel (for composition queries)
    /// </summary>
    public Guid CourseId { get; set; }

    // Core Metadata
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }

    // Stats (denormalized for quick access)
    public int LessonCount { get; set; }
    public double TotalDurationSeconds { get; set; }

    // Timestamps
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
