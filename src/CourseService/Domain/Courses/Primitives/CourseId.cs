namespace Courses.Domain.Courses.Primitives;

public record struct CourseId(Guid Value)
{
    public static CourseId CreateNew() => new(Guid.CreateVersion7());
}