using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CourseListItemReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string? InstructorAvatarUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Language { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public int LessonsCount { get; set; }
    public double TotalDurationSeconds { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public string? Slug { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> Tags { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
}
