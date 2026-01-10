using Kernel;
using System.Reflection;

namespace Courses.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for mapping Value Objects in minimal API endpoints.
/// Provides automatic parameter conversion for ISingleValueObject types without modifying the domain.
/// </summary>
public static class ValueObjectMappingExtensions
{
    /// <summary>
    /// Maps a Guid parameter to a Value Object that implements ISingleValueObject&lt;Guid&gt;.
    /// Usage: MapValueObject&lt;CourseId&gt;(id)
    /// </summary>
    public static T MapValueObject<T>(this Guid value) where T : struct, ISingleValueObject<Guid>
    {
        var constructor = typeof(T).GetConstructor(new[] { typeof(Guid) });
        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Type {typeof(T).Name} must have a constructor that accepts Guid");
        }

        return (T)constructor.Invoke(new object[] { value });
    }

    /// <summary>
    /// Maps a string parameter to a Value Object that implements ISingleValueObject&lt;string&gt;.
    /// Usage: MapValueObject&lt;Title&gt;(title)
    /// </summary>
    public static T MapValueObject<T>(this string value) where T : struct, ISingleValueObject<string>
    {
        var constructor = typeof(T).GetConstructor(new[] { typeof(string) });
        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Type {typeof(T).Name} must have a constructor that accepts string");
        }

        return (T)constructor.Invoke(new object[] { value });
    }
}
