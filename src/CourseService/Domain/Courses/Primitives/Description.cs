namespace Courses.Domain.Courses.Primitives;

public record Description(string Value)
{
    public static Description Empty => new Description(string.Empty);
}
