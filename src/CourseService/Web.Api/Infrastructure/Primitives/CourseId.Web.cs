//using System.Diagnostics.CodeAnalysis;

//namespace Courses.Domain.Courses.Primitives;

//public partial record struct CourseId(Guid Value) : IParsable<CourseId>
//{
//    public static CourseId Parse(string s, IFormatProvider? provider)
//    {
//        if (TryParse(s, provider, out var result))
//        {
//            return result;
//        }

//        throw new FormatException($"String '{s}' was not in a correct format for CourseId.");
//    }

//    public static bool TryParse([NotNullWhen(true)] string? s, out CourseId result)
//        => TryParse(s, null, out result);

//    public static bool TryParse(
//        [NotNullWhen(true)] string? s,
//        IFormatProvider? provider,
//        [MaybeNullWhen(false)] out CourseId result)
//    {
//        if (Guid.TryParse(s, out var guid))
//        {
//            result = new CourseId(guid);
//            return true;
//        }

//        result = default;
//        return false;
//    }
//}