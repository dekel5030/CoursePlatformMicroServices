namespace Courses.Domain.Lessons.Primitives;

public record LessonId(Guid Value)
{
    public static LessonId CreateNew() => new(Guid.CreateVersion7());
};