using Kernel;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Lessons;

public class Lesson : Entity
{
    public LessonId Id { get; private set; } = LessonId.CreateNew();
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public LessonAccess Access { get; private set; } = LessonAccess.Private;
    public LessonStatus Status { get; private set; } = LessonStatus.Draft;

    public CourseId CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    public int Index { get; private set; } = 0;

    public ImageUrl? ThumbnailImageUrl { get; private set; }
    public VideoUrl? VideoUrl { get; private set; }
    public TimeSpan? Duration { get; private set; }

    private Lesson() { }

    internal static Result<Lesson> Create(
        CourseId courseId,
        Title? title,
        Description? description,
        int index = 0)
    {
        Lesson newLesson = new Lesson()
        {
            Title = title ?? Title.Empty,
            Description = description ?? Description.Empty,
            Index = index,
        };

        return Result.Success(newLesson);
    }
    
    public Result UpdateVideoData(VideoUrl videoUrl, TimeSpan duration)
    {
        VideoUrl = videoUrl;
        Duration = duration;

        Raise(new LessonVideoDataUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result SetThumbnailImage(ImageUrl url)
    {
        ThumbnailImageUrl = url;

        return Result.Success();
    }

    public Result SetDescription(Description description)
    {
        Description = description;
        return Result.Success();
    }

    public Result SetTitle(Title title)
    {
        Title = title;

        return Result.Success();
    }

    public Result SetAccess(LessonAccess access)
    {
        Access = access;
        return Result.Success();
    }

    internal Result SetIndex(int index)
    {
        Index = index;
        return Result.Success();
    }
}

public record LessonVideoDataUpdatedDomainEvent(Lesson Lesson) : IDomainEvent;