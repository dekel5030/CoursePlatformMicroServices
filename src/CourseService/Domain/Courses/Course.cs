using Courses.Domain.Courses.Events;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Courses.Domain.Users;
using Kernel;

namespace Courses.Domain.Courses;

public class Course : Entity<CourseId>
{
    public override CourseId Id { get; protected set; }
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public Money Price { get; private set; } = Money.Zero();
    public DifficultyLevel Difficulty { get; private set; } = DifficultyLevel.Beginner;
    public CourseCategory Category { get; private set; } = CourseCategory.Unkown;
    public Language Language { get; private set; } = Language.Hebrew;
    public Slug Slug { get; private set; }

    public UserId InstructorId { get; private set; }
    public User? Instructor { get; private set; }
    
    public int EnrollmentCount { get; private set; }
    public int LessonCount { get; private set; }
    public TimeSpan TotalDuration { get; private set; } = TimeSpan.Zero;

    public IReadOnlyList<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyList<ImageUrl> Images => _images.AsReadOnly();
    public IReadOnlyCollection<Tag> Tags => _tags;
    public DateTimeOffset UpdatedAtUtc { get; private set; }


    private readonly List<Lesson> _lessons = new();
    private readonly List<ImageUrl> _images = new();
    private readonly HashSet<Tag> _tags = new();

#pragma warning disable S1133
#pragma warning disable CS8618 
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private Course() { }
#pragma warning restore CS8618 
#pragma warning restore S1133 

    private Course(CourseId id, UserId instructorId, Slug slug)
    {
        Id = id;
        InstructorId = instructorId;
        Slug = slug;
    }

    public static Result<Course> CreateCourse(
        UserId instructorId,
        Title? title = null,
        Description? description = null,
        Money? price = null)
    {
        var courseId = CourseId.CreateNew();
        var slug = new Slug(courseId.ToString());
        var newCourse = new Course(courseId, instructorId, slug)
        {
            Title = title ?? Title.Empty,
            Description = description ?? Description.Empty,
            InstructorId = instructorId,
            Status = CourseStatus.Draft,
            Price = price ?? Money.Zero(),
        };

        return Result.Success(newCourse);
    }

    public Result CanModify => CoursePolicies.CanModify(Status);
    public Result CanDelete => CoursePolicies.CanDelete(Status);
    public Result CanEnroll => CoursePolicies.CanEnroll(Status);
    public Result CanPublish => CoursePolicies.CanPublish(Status, LessonCount);

    public Result Publish()
    {
        if (CanPublish.IsFailure)
        {
            return CanPublish;
        }

        Status = CourseStatus.Published;

        Raise(new CoursePublished(this));

        return Result.Success();
    }

    public Result<Lesson> AddLesson(
        Title? title,
        Description? description)
    {
        int index = _lessons.Count;

        Result<Lesson> lessonResult = Lesson.Create(Id ,title, description, index);

        if (lessonResult.IsFailure)
        {
            return Result.Failure<Lesson>(lessonResult.Error);
        }

        Lesson lesson = lessonResult.Value;

        _lessons.Add(lesson);
        LessonCount++;

        return Result.Success(lesson);
    }

    public Result RemoveLesson(LessonId lessonId)
    {
        Lesson? lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);

        if (lesson is null)
        {
            return Result.Failure<Course>(LessonErrors.NotFound);
        }

        _lessons.Remove(lesson);
        LessonCount--;
        Raise(new CourseLessonDeleted(this, lesson));

        return Result.Success();
    }

    public Result UpdateLesson(
        LessonId lessonId,
        Title? title = null,
        Description? description = null,
        LessonAccess? access = null,
        int? index = null,
        Slug? slug = null)
    {
        Result policyResult = CanModify;
        if (policyResult.IsFailure)
        {
            return policyResult;
        }

        Lesson? lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        lesson.UpdateDetails(title, description, access, index, slug);

        return Result.Success();
    }

    public Result UpdateLessonMedia(
        LessonId lessonId,
        VideoUrl? videoUrl = null,
        ImageUrl? imageUrl = null,
        TimeSpan? duration = null)
    {
        Lesson? lesson = _lessons.FirstOrDefault(x => x.Id == lessonId);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        TimeSpan previousDuration = lesson.Duration;
        Result updateResult = lesson.UpdateMedia(imageUrl, videoUrl, duration);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        TotalDuration = TotalDuration - previousDuration + lesson.Duration;

        return Result.Success();
    }

    public Result AddImage(ImageUrl imageUrl)
    {
        if (_images.Contains(imageUrl))
        {
            return Result.Success();
        }

        _images.Add(imageUrl);

        return Result.Success();
    }

    public Result RemoveImage(ImageUrl imageUrl)
    {
        _images.Remove(imageUrl);
        return Result.Success();
    }

    public Result UpdateDetails(
        Title? title = null,
        Description? description = null,
        Money? price = null)
    {
        if (CanModify.IsFailure)
        {
            return CanModify;
        }

        if (title is not null && title != Title)
        {
            Title = title;
        }

        if (description is not null && description != Description)
        {
            Description = description;
        }

        if (price is not null && price != Price)
        {
            Price = price;
        }

        return Result.Success();
    }

    public Result UpdateMetadata(
        DifficultyLevel? difficulty = null,
        CourseCategory? category = null,
        Language? language = null,
        ICollection<Tag>? tags = null,
        Slug? slug = null)
    {
        if (CanModify.IsFailure)
        {
            return CanModify;
        }

        if (difficulty is not null && difficulty != Difficulty)
        {
            Difficulty = difficulty.Value;
        }

        if (category is not null && category != Category)
        {
            Category = category;
        }

        if (language is not null && language != Language)
        {
            Language = language;
        }

        if (slug is not null && slug != Slug)
        {
            Slug = slug;
        }

        if (tags is not null)
        {
            _tags.Clear();
            foreach (Tag tag in tags)
            {
                _tags.Add(tag);
            }
        }

        return Result.Success();
    }

    public Result Delete()
    {
        if (CanDelete.IsFailure)
        {
            return CanDelete;
        }

        Status = CourseStatus.Deleted;
        Raise(new CourseDeleted(this));
        return Result.Success();
    }

    internal Result Enroll()
    {
        if (CanEnroll.IsFailure)
        {
            return CanEnroll;
        }

        EnrollmentCount++;
        return Result.Success();
    }
}
