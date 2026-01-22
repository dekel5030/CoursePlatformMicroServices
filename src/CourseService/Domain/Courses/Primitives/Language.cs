namespace Courses.Domain.Courses.Primitives;

public sealed record Language
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

    public static Language Parse(string code) => new(code);

    public static readonly Language English = new("en");
    public static readonly Language Hebrew = new("he");
}
