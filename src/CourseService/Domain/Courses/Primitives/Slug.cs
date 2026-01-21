using System.Text.RegularExpressions;
using Kernel;

namespace Courses.Domain.Courses.Primitives;

public sealed record Slug
{
    public string Value { get; }

    public Slug(string raw)
    {
        string str = raw.ToLowerInvariant();

        str = Regex.Replace(str, @"[^\p{L}0-9\s-]", "");
        str = Regex.Replace(str, @"[\s-]+", " ").Trim();
        str = Regex.Replace(str, @"\s", "-");

        Value = str;
    }
}
