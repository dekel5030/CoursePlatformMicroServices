using Kernel;

namespace Courses.Domain.CourseViews.Primitives;

public record CourseViewId(Guid Value) : ISingleValueObject<Guid>
{
    public static CourseViewId CreateNew() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
