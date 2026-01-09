using Kernel;

namespace Courses.Domain.Shared.Primitives;

public record struct Description(string Value) : ISingleValueObject<string>
{
    public static Description Empty => new Description(string.Empty);
}
