using Kernel;

namespace Courses.Domain.Lessons.Primitives;

public record struct LessonId(Guid Value) : ISingleValueObject<Guid>
{
    public static LessonId CreateNew() => new(Guid.CreateVersion7());
}

