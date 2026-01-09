using Kernel;

namespace Courses.Domain.Courses.Primitives;

public record InstructorId(Guid Value) : ISingleValueObject<Guid>;
