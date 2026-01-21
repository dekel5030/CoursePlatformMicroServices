using Kernel;

namespace Courses.Domain.Lessons.Primitives;

public sealed record LessonId(Guid Value) : ISingleValueObject<Guid>
{
    public static LessonId CreateNew() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}

