using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Events;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments;
using Courses.Domain.Lessons;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public class Course : Entity
{
    private readonly List<Lesson> _lessons = new();
    private readonly List<ImageUrl> _images = new();
    private readonly List<Enrollment> _enrollments = new();

    public CourseId Id { get; private set; }
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public InstructorId? InstructorId { get; private set; } = null;
    public CourseStatus Status { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyCollection<ImageUrl> Images => _images.AsReadOnly();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

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

    public Result Publish(TimeProvider timeProvider)
    {
        if (Status == CourseStatus.Published)
        {
            return Result.Failure<Course>(CourseErrors.AlreadyPublished);
        }
        if (_lessons.Count == 0)
        {
            return Result.Failure<Course>(CourseErrors.CourseWithoutLessons);
        }

        Status = CourseStatus.Published;
        UpdatedAtUtc = timeProvider.GetUtcNow();

        Raise(new CoursePublished(this));

        return Result.Success();
    }

    public Result AddLesson(Lesson lesson, TimeProvider timeProvider)
    {
        if (_lessons.Any(l => l.Id == lesson.Id))
        {
            return Result.Failure<Course>(CourseErrors.LessonAlreadyExists);
        }
        _lessons.Add(lesson);
        UpdatedAtUtc = timeProvider.GetUtcNow();
        return Result.Success();
    }

    public Result RemoveLesson(Lesson lesson, TimeProvider timeProvider)
    {
        if (_lessons.Remove(lesson))
        {
            UpdatedAtUtc = timeProvider.GetUtcNow();
        }

        return Result.Success();
    }

    public Result SetPrice(Money price, TimeProvider timeProvider)
    {
        if (price.Amount < 0)
        {
            return Result.Failure<Course>(CourseErrors.InvalidPrice);
        }
        Price = price;
        UpdatedAtUtc = timeProvider.GetUtcNow();
        return Result.Success();
    }

    public Result AddImage(ImageUrl imageUrl, TimeProvider timeProvider)
    {
        if (_images.Contains(imageUrl))
        {
            return Result.Failure<Course>(CourseErrors.ImageAlreadyExists);
        }
        _images.Add(imageUrl);
        UpdatedAtUtc = timeProvider.GetUtcNow();
        return Result.Success();
    }

    public Result RemoveImage(ImageUrl imageUrl, TimeProvider timeProvider)
    {
        if (_images.Remove(imageUrl))
        {
            UpdatedAtUtc = timeProvider.GetUtcNow();
        }

        return Result.Success();
    }

    public Result UpdateDescription(Description description, TimeProvider timeProvider)
    {
        Description = description;
        UpdatedAtUtc = timeProvider.GetUtcNow();

        return Result.Success();
    }

    public Result UpdateTitle(Title title, TimeProvider timeProvider)
    {
        Title = title;
        UpdatedAtUtc = timeProvider.GetUtcNow();
        return Result.Success();
    }

    public Result AssignInstructor(InstructorId instructorId, TimeProvider timeProvider)
    {
        InstructorId = instructorId;
        UpdatedAtUtc = timeProvider.GetUtcNow();

        return Result.Success();
    }

    public Result Enroll(StudentId studentId, TimeProvider timeProvider)
    {
        if (Status != CourseStatus.Published)
        {
            return Result.Failure<Course>(CourseErrors.CourseNotPublished);
        }

        if (_enrollments.Any(e => e.StudentId == studentId))
        {
            return Result.Success();
        }

        Enrollment enrollment = Enrollment.CreateEnrollment(Id, studentId, timeProvider);
        _enrollments.Add(enrollment);
        UpdatedAtUtc = timeProvider.GetUtcNow();

        Raise(new StudentEnrolled(this, studentId));

        return Result.Success();
    }
}