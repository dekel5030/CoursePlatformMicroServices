namespace Courses.Domain.Courses.Primitives;

public sealed record Tag
{
    public string Value { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Tag() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Tag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Tag cannot be empty");
        }

        if (value.Length > 30)
        {
            throw new ArgumentException("Tag too long");
        }

        Value = value.ToLowerInvariant();
    }

    public static Tag Create(string value) => new(value);
}
