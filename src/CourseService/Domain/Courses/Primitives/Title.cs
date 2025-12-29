namespace Courses.Domain.Courses.Primitives;

public record Title(string Value)
{
    public static Title Empty => new Title(string.Empty);
}
