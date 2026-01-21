using System.Text.RegularExpressions;
using Kernel;

namespace Courses.Domain.Courses.Primitives;

public readonly record struct Slug
{
    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Result<Slug> Create(string instructorName, string courseTitle)
    {
        string rawSlug = $"{instructorName}-{courseTitle}";

        if (string.IsNullOrWhiteSpace(rawSlug))
        {
            return Result.Failure<Slug>(Error.Validation("Slug.Cannot.BeEmpty", "Slug cannot be empty."));
        }

        string str = rawSlug.ToLowerInvariant();

        str = Regex.Replace(str, @"[^\p{L}0-9\s-]", "");
        str = Regex.Replace(str, @"[\s-]+", " ").Trim();
        str = Regex.Replace(str, @"\s", "-");

        if (string.IsNullOrWhiteSpace(str))
        {
            return Result.Failure<Slug>(Error.Validation("Slug.Invalid", "Slug is invalid"));
        }

        return Result.Success(new Slug(str));
    }
}
