namespace Courses.Domain.Courses.Primitives;

public readonly record struct Language
{
    public string Code { get; }
    private Language(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Language code is required");
        }

        Code = code;
    }

    public static readonly Language English = new("en");
    public static readonly Language Hebrew = new("he");
}
