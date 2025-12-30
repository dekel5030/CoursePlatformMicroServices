namespace Courses.Domain.Shared.Primitives;

public record Description(string Value)
{
    public static Description Empty => new Description(string.Empty);
}
