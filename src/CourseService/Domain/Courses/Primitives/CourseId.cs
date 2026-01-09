using System.Diagnostics.CodeAnalysis;
using Kernel;

namespace Courses.Domain.Courses.Primitives;

public record struct CourseId(Guid Value) : ISingleValueObject<Guid>
{
    public static CourseId CreateNew() => new(Guid.CreateVersion7());
}