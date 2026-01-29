using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Application.Courses.ReadModels;

public sealed class CoursePage
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
    public int EnrollmentCount { get; set; }
    public int LessonsCount { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }

    public string? Slug { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
    public List<string> ImageUrls { get; set; } = [];
    public List<string> Tags { get; set; } = [];

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;

    public List<ModuleReadModel> Modules { get; set; } = [];

#pragma warning restore CA2227 // Collection properties should be read only
    public CoursePageDto ToDto()
    {
        return new CoursePageDto
        {
            Id = Id,
            Title = Title,
            Description = Description,
            InstructorId = InstructorId,
            InstructorName = InstructorName,
            InstructorAvatarUrl = InstructorAvatarUrl,
            Status = Status,
            Price = new Money(PriceAmount, PriceCurrency),
            EnrollmentCount = EnrollmentCount,
            LessonsCount = LessonsCount,
            TotalDuration = Duration,
            UpdatedAtUtc = UpdatedAtUtc,
            ImageUrls = ImageUrls.AsReadOnly(),
            Tags = Tags.AsReadOnly(),
            CategoryId = CategoryId,
            CategoryName = CategoryName,
            CategorySlug = CategorySlug,
            Modules = Modules.ConvertAll(m => m.ToDto()),
            Links = []
        };
    }
}
