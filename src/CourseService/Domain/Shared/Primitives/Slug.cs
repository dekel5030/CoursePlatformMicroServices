using System.Text.RegularExpressions;

namespace Courses.Domain.Shared.Primitives;

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
