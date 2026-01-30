using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

/// <summary>
/// Core Read Model for Course aggregate.
/// Contains all course-level metadata without nested collections.
/// Can be composed with ModuleReadModel and LessonReadModel to build any DTO.
/// </summary>
public sealed class CourseReadModel
{
    /// <summary>
    /// Primary Key - CourseId (Point Lookup)
    /// </summary>
    public Guid Id { get; set; }

    // Core Metadata
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public CourseStatus Status { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Language { get; set; } = string.Empty;

    // Pricing
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = string.Empty;

    // Instructor (FK to InstructorReadModel)
    public Guid InstructorId { get; set; }

    // Category
    public Guid CategoryId { get; set; }

    // Media
#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> ImageUrls { get; set; } = [];
    public List<string> Tags { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only

    // Stats (denormalized for list queries)
    public int TotalModules { get; set; }
    public int TotalLessons { get; set; }
    public double TotalDurationSeconds { get; set; }
    public int EnrollmentCount { get; set; }

    // Timestamps
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
