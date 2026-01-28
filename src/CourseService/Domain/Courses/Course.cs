using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Events;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public class Course : Entity<CourseId>, ICourseSnapshot
{
    public override CourseId Id { get; protected set; }
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public CourseStatus Status { get; private set; } = CourseStatus.Draft;
    public DifficultyLevel Difficulty { get; private set; } = DifficultyLevel.Beginner;
    public Money Price { get; private set; } = Money.Zero();
    public Language Language { get; private set; } = Language.Hebrew;
    public Slug Slug { get; private set; }

    public UserId InstructorId { get; private set; }
    public CategoryId CategoryId { get; private set; } = new CategoryId(Guid.Empty);

    public IReadOnlyCollection<Tag> Tags => _tags;
    public IReadOnlyCollection<ImageUrl> Images => _images;
    public DateTimeOffset UpdatedAtUtc { get; private set; }

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

        newCourse.Raise(new CourseCreatedDomainEvent(newCourse));

        return Result.Success(newCourse);
    }

    public Result CanModify => CoursePolicies.CanModify(Status);
    public Result CanDelete => CoursePolicies.CanDelete(Status);
    public Result CanEnroll => CoursePolicies.CanEnroll(Status);
    public Result CanPublish => CoursePolicies.CanPublish(Status);

    public Result Publish()
    {
        if (CanPublish.IsFailure)
        {
            return CanPublish;
        }

        Status = CourseStatus.Published;

        Raise(new CoursePublishedDomainEvent(this));

        return Result.Success();
    }

    public Result AddImage(ImageUrl imageUrl)
    {
        if (_images.Contains(imageUrl))
        {
            return Result.Success();
        }

        _images.Add(imageUrl);

        Raise(new CourseUpdatedDomainEvent(this));
        return Result.Success();
    }

    public Result RemoveImage(ImageUrl imageUrl)
    {
        _images.Remove(imageUrl);

        Raise(new CourseUpdatedDomainEvent(this));
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

        Raise(new CourseUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result UpdateMetadata(
        DifficultyLevel? difficulty = null,
        CategoryId? categoryId = null,
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

        if (categoryId is not null && categoryId != CategoryId)
        {
            CategoryId = categoryId;
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

        Raise(new CourseUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result Delete()
    {
        if (CanDelete.IsFailure)
        {
            return CanDelete;
        }

        Status = CourseStatus.Deleted;

        Raise(new CourseDeletedDomainEvent(this));

        return Result.Success();
    }
}
