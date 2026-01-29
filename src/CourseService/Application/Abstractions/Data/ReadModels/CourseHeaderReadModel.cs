using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CourseHeaderReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string? InstructorAvatarUrl { get; set; }
    public CourseStatus Status { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Language { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public string? Slug { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> ImageUrls { get; set; } = [];
    public List<string> Tags { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
}
