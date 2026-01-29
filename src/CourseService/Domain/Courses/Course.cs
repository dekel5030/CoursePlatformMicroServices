using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Events;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public class Course : Entity<CourseId>
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
        var course = new Course(courseId, instructorId, slug)
        {
            Title = title ?? Title.Empty,
            Description = description ?? Description.Empty,
            InstructorId = instructorId,
            Status = CourseStatus.Draft,
            Price = price ?? Money.Zero(),
        };

        course.Raise(new CourseCreatedDomainEvent(
            course.Id,
            course.InstructorId,
            course.Title,
            course.Description,
            course.Price,
            course.Status,
            course.Slug,
            course.Difficulty,
            course.Language,
            course.CategoryId
        ));

        return Result.Success(course);
    }

    public Result CanModify => CoursePolicies.CanModify(Status);
    public Result CanDelete => CoursePolicies.CanDelete(Status);
    public Result CanEnroll => CoursePolicies.CanEnroll(Status);
    public Result CanPublish => CoursePolicies.CanPublish(Status);

    #region Specific Update Methods

    public Result ChangeTitle(Title title)
    {
        if (CanModify.IsFailure)
        {
            return CanModify;
        }

        if (Title == title)
        {
            return Result.Success();
        }

        Title = title;
        Raise(new CourseTitleChangedDomainEvent(Id, Title));
        return Result.Success();
    }

    public Result ChangeDescription(Description description)
    {
        if (CanModify.IsFailure)
        {
            return CanModify;
        }

        if (Description == description)
        {
            return Result.Success();
        }

        Description = description;
        Raise(new CourseDescriptionChangedDomainEvent(Id, Description));
        return Result.Success();
    }

    public Result ChangePrice(Money price)
    {
        if (CanModify.IsFailure)
        {
            return CanModify;
        }

        if (Price == price)
        {
            return Result.Success();
        }

        Price = price;
        Raise(new CoursePriceChangedDomainEvent(Id, Price));
        return Result.Success();
    }
    #endregion

    public Result UpdateDetails(
        Title? title = null,
        Description? description = null,
        Money? price = null)
    {
        if (title is not null)
        {
            ChangeTitle(title);
        }

        if (description is not null)
        {
            ChangeDescription(description);
        }

        if (price is not null)
        {
            ChangePrice(price);
        }

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
            Raise(new CourseDifficultyChangedDomainEvent(Id, Difficulty));
        }

        if (categoryId is not null && categoryId != CategoryId)
        {
            CategoryId = categoryId;
            Raise(new CourseDomainEvents(Id, CategoryId));
        }

        if (language is not null && language != Language)
        {
            Language = language;
            Raise(new CourseLanguageChangedDomainEvent(Id, Language));
        }

        if (slug is not null && slug != Slug)
        {
            Slug = slug;
            Raise(new CourseSlugChangedDomainEvent(Id, Slug));
        }

        if (tags is not null)
        {
            _tags.Clear();
            foreach (Tag tag in tags)
            {
                _tags.Add(tag);
            }

            Raise(new CourseTagsUpdatedDomainEvent(Id, Tags));
        }

        return Result.Success();
    }

    public Result Publish()
    {
        if (CanPublish.IsFailure)
        {
            return CanPublish;
        }

        Status = CourseStatus.Published;
        Raise(new CourseStatusChangedDomainEvent(Id, Status));

        return Result.Success();
    }

    public Result AddImage(ImageUrl imageUrl)
    {
        if (_images.Contains(imageUrl))
        {
            return Result.Success();
        }

        _images.Add(imageUrl);
        Raise(new CourseImageAddedDomainEvent(Id, imageUrl));
        return Result.Success();
    }

    public Result RemoveImage(ImageUrl imageUrl)
    {
        if (!_images.Contains(imageUrl))
        {
            return Result.Success();
        }

        _images.Remove(imageUrl);
        Raise(new CourseImageRemovedDomainEvent(Id, imageUrl));
        return Result.Success();
    }

    public Result Delete()
    {
        if (CanDelete.IsFailure)
        {
            return CanDelete;
        }

        Status = CourseStatus.Deleted;
        Raise(new CourseStatusChangedDomainEvent(Id, Status));

        return Result.Success();
    }
}
