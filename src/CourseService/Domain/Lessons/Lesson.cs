using Kernel;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Shared.Primitives;

namespace Courses.Domain.Lessons;

public class Lesson : Entity
{
    public LessonId Id { get; private set; } = new LessonId(Guid.NewGuid());
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public LessonAccess Access { get; private set; } = LessonAccess.Private;
    public LessonStatus Status { get; private set; } = LessonStatus.Draft;

    public CourseId CourseId { get; private set; }
    public Course? Course { get; private set; }

    public int Index { get; private set; } = 0;

    public ImageUrl? ThumbnailImageUrl { get; private set; }
    public VideoUrl? VideoUrl { get; private set; }
    public TimeSpan? Duration { get; private set; }

    private Lesson() { }

    internal static Result<Lesson> Create(
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
}

