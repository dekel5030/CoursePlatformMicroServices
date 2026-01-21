namespace Courses.Domain.Courses;

public readonly record struct Tag
{
    public string Value { get; }

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
