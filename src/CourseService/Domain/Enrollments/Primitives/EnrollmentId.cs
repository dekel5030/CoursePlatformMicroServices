using Kernel;

namespace Courses.Domain.Enrollments.Primitives;

public record EnrollmentId(Guid Value) : ISingleValueObject<Guid>
{
    public static EnrollmentId CreateNew() => new(Guid.NewGuid());
}
