using System.Diagnostics.CodeAnalysis;
using Kernel;

namespace Courses.Domain.Lessons.Primitives;

public record struct LessonId(Guid Value) : ISingleValueObject<LessonId, Guid>
{
    public static LessonId CreateNew() => new(Guid.CreateVersion7());

    public static LessonId Parse(string s, IFormatProvider? provider)
    {
        if (LessonId.TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"String '{s}' was not in a correct format for LessonId.");
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out LessonId result)
        => TryParse(s, null, out result);

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out LessonId result)
    {
        if (Guid.TryParse(s, out var guid))
        {
            result = new LessonId(guid);
            return true;
        }

        result = default;
        return false;
    }
}

