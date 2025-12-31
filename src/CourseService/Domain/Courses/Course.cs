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
    public CourseId Id { get; private set; }
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public InstructorId? InstructorId { get; private set; } = null;
    public CourseStatus Status { get; private set; }
    public int EnrollmentCount { get; private set; } = 0;

    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyCollection<ImageUrl> Images => _images.AsReadOnly();

    #pragma warning disable CS8618
    private Course() { }
    #pragma warning restore CS8618

    public static Result<Course> CreateCourse(
        TimeProvider timeProvider,
        Title? title = null,
        Description? description = null,
        InstructorId? instructorId = null,
        Money? price = null)
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
            return Result.Success();
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

    public Result<Enrollment> CreateEnrollment(
        StudentId studentId, 
        TimeProvider timeProvider, 
        TimeSpan validFor)
    {
        if (Status != CourseStatus.Published)
        {
            return Result.Failure<Enrollment>(CourseErrors.CourseNotPublished);
        }

        var enrollment = Enrollment.Create(Id, studentId, timeProvider, validFor);
        EnrollmentCount++;

        return Result.Success(enrollment);
    }

    public Result<Lesson> AddLesson(
        Title? title,
        Description? description,
        TimeProvider timeProvider)
    {
        int index = _lessons.Count; 
        
        var lessonResult = Lesson.Create(Id, title, description, index);

        if (lessonResult.IsFailure)
        {
            return Result.Failure<Lesson>(lessonResult.Error);
        }

        var lesson = lessonResult.Value;
        
        _lessons.Add(lesson);
        UpdatedAtUtc = timeProvider.GetUtcNow();

        return Result.Success(lesson);
    }
}