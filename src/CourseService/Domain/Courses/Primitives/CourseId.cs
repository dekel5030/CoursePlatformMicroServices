using Kernel;

namespace Courses.Domain.Courses.Primitives;

public record struct CourseId(Guid Value) : ISingleValueObject<Guid>
{
    public static CourseId CreateNew() => new(Guid.CreateVersion7());
    public override string ToString()
    {
        return Value.ToString();
    }
}
