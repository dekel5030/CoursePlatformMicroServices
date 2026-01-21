using Kernel;

namespace Courses.Domain.Shared.Primitives;

public record Title(string Value) : ISingleValueObject<string>
{
    public static Title Empty => new Title(string.Empty);
}
