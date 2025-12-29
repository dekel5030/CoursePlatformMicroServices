using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Kernel;

namespace Courses.Domain.Courses;

public class Course : Entity
{
    private readonly List<Lesson> _lessons = new();
    private readonly List<ImageUrl> _images = new();

    public CourseId Id { get; private set; }
    public Title Title { get; set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public InstructorId? InstructorId { get; set; } = null;
    public CourseStatus Status { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyCollection<ImageUrl> Images => _images.AsReadOnly();


    #pragma warning disable CS8618
    private Course() { }
    #pragma warning restore CS8618

    public static Result<Course> CreateCourse(
        Title? title,
        Description? description,
        InstructorId? instructorId,
        Money? price,
        TimeProvider timeProvider)
    {
        Course newCourse = new Course
        {
            Id = CourseId.CreateNew(),
            Title = title ?? Title.Empty,
            Description = description ?? Description.Empty,
            InstructorId = instructorId,
            Status = CourseStatus.Draft,
            Price = price ?? Money.Zero(),
            UpdatedAtUtc = timeProvider.GetUtcNow()
        };

        return Result.Success(newCourse);
    }
}
