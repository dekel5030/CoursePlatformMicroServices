using Kernel;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Courses;

namespace Courses.Domain.Lessons;

public class Lesson : Entity
{
    private Lesson() { }

    public LessonId Id { get; private set; } = new LessonId(Guid.NewGuid());
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public string? ThumbnailImage { get; set; }
    public bool IsPreview { get; set; } = false;

    public int Order { get; set; } = 1;
    public TimeSpan? Duration { get; set; }

    public CourseId CourseId { get; set; }
    public Course? Course { get; set; }

    public static Lesson CreateLesson(
        string? title,
        string? description,
        string? videoUrl,
        string? thumbnailUrl,
        bool isPreview,
        int order,
        TimeSpan? duration)
    {
        return new Lesson()
        {
            Id = new LessonId(Guid.CreateVersion7()),
            Title = title ?? "Untitled Lesson",
            Description = description,
            VideoUrl = videoUrl,
            ThumbnailImage = thumbnailUrl,
            IsPreview = isPreview,
            Order = order,
            Duration = duration
        };
    }
}
