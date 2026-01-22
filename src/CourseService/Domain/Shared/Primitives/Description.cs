using Kernel;

namespace Courses.Domain.Shared.Primitives;

public record Description(string Value) : ISingleValueObject<string>
{
    public static Description Empty => new Description(string.Empty);
}
