using System;
using System.ComponentModel;
using Courses.Domain.Courses.Errors;
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
    private readonly List<Lesson> _lessons = new();
    private readonly List<ImageUrl> _images = new();
    private readonly HashSet<Tag> _tags = new();

    public override CourseId Id { get; protected set; }
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public UserId InstructorId { get; private set; }
    public User? Instructor { get; private set; }
    public CourseStatus Status { get; private set; }
    public int EnrollmentCount { get; private set; }
    public int LessonCount { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public IReadOnlyList<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyList<ImageUrl> Images => _images.AsReadOnly();

    public DifficultyLevel Difficulty { get; private set; }
    public CourseCategory Category { get; private set; }
    public Language Language { get; private set; }
    public TimeSpan TotalDuration { get; private set; }

    public Slug Slug { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags;


    #pragma warning disable CS8618
    private Course() { }
    #pragma warning restore CS8618

    public static Result<Course> CreateCourse(
        TimeProvider timeProvider,
        UserId instructorId,
        Title? title = null,
        Description? description = null,
        Money? price = null)
    {
        var newCourse = new Course
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

    public Result CanModify => CoursePolicies.CanModify(Status);
    public Result CanDelete => CoursePolicies.CanDelete(Status);
    public Result CanEnroll => CoursePolicies.CanEnroll(Status);
    public Result CanPublish => CoursePolicies.CanPublish(Status, LessonCount);

    public Result Publish(TimeProvider timeProvider)
    {
        if (CanPublish.IsFailure)
        {
            return CanPublish;
        }

        Status = CourseStatus.Published;
        UpdatedAtUtc = timeProvider.GetUtcNow();

        Raise(new CoursePublished(this));

        return Result.Success();
    }

    public Result<Lesson> AddLesson(
        Title? title,
        Description? description,
        TimeProvider timeProvider)
    {
        int index = _lessons.Count;

        Result<Lesson> lessonResult = Lesson.Create(title, description, index);

        if (lessonResult.IsFailure)
        {
            return Result.Failure<Lesson>(lessonResult.Error);
        }

        Lesson lesson = lessonResult.Value;

        _lessons.Add(lesson);
        LessonCount++;
        UpdatedAtUtc = timeProvider.GetUtcNow();

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
        Title? title,
        Description? description,
        LessonAccess? access,
        TimeProvider timeProvider)
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

        if (title.HasValue)
        {
            lesson.SetTitle(title.Value);
        }

        if (description.HasValue)
        {
            lesson.SetDescription(description.Value);
        }

        if (access.HasValue)
        {
            lesson.SetAccess(access.Value);
        }

        UpdatedAtUtc = timeProvider.GetUtcNow();

        return Result.Success();
    }

    public Result UpdateLessonVideo(LessonId lessonId, VideoUrl url, TimeSpan duration)
    {
        Lesson? lesson = _lessons.FirstOrDefault(x => x.Id == lessonId);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        TimeSpan previousDuration = lesson.Duration;
        Result updateResult = lesson.UpdateVideoData(url, duration);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        TotalDuration = TotalDuration - previousDuration + lesson.Duration;

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

    public Result UpdateDetails(
        Title? title = null,
        Description? description = null,
        Money? price = null)
    {
        if (CanModify.IsFailure)
        {
            return CanModify;
        }

        if (title.HasValue && title.Value != Title)
        {
            Title = title.Value;
        }

        if (description.HasValue && description.Value != Description)
        {
            Description = description.Value;
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

        if (difficulty.HasValue && difficulty.Value != Difficulty)
        {
            Difficulty = difficulty.Value;
        }

        if (category.HasValue && category.Value != Category)
        {
            Category = category.Value;
        }

        if (language.HasValue && language.Value != Language)
        {
            Language = language.Value;
        }

        if (slug.HasValue && slug.Value != Slug)
        {
            Slug = slug.Value;
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
