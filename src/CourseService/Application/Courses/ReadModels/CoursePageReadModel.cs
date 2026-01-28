using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.ReadModels;

public sealed class CoursePageReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string? InstructorAvatarUrl { get; set; }
    public CourseStatus Status { get; set; }
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public int LessonsCount { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }

    public List<string> ImageUrls { get; private set; } = new();
    public List<string> Tags { get; private set; } = new();

    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;

    public List<ModuleReadModel> Modules { get; private set; } = new();

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
