using Domain.Courses.Primitives;
using Domain.Lessons;
using Kernel;

namespace Domain.Courses;

public class Course : Entity
{
    private Course() { }

    public CourseId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }

    public string? InstructorUserId { get; private set; }

    public bool IsPublished { get; private set; } = false;

    public Money Price { get; private set; } = Money.Zero();

    public ICollection<Lesson> Lessons { get; private set; } = new List<Lesson>();
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Course CreateCourse(
        string? title,
        string? description,
        string? imageUrl,
        string? instructorUserId,
        Money? price)
    {
        return new Course()
        {
            Id = new CourseId(Guid.CreateVersion7()),
            Title = title ?? $"Unnamed course",
            Description = description ?? string.Empty,
            ImageUrl = imageUrl,
            InstructorUserId = instructorUserId,
            IsPublished = false,
            Price = price ?? Money.Zero(),
            Lessons = new List<Lesson>(),
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
