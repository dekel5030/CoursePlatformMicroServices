using Kernel;

namespace Courses.Domain.Courses.Primitives;

public record StudentId(Guid Value) : ISingleValueObject<Guid>;