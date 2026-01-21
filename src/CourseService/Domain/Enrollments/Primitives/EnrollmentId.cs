namespace Courses.Domain.Enrollments.Primitives;

public record EnrollmentId(Guid Value)
{
    public static EnrollmentId CreateNew() => new(Guid.NewGuid());
    public override string ToString()
    {
        return Value.ToString();
    }
}
