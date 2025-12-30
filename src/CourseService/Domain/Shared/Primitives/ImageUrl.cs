namespace Courses.Domain.Shared.Primitives;

public sealed record ImageUrl
{
    private static readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public Url Value { get; }

    public string Path => Value.Path;

    private ImageUrl(Url value)
    {
        Value = value;
    }

    public static ImageUrl Create(string path)
    {
        if (!_allowedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Invalid image format");

        return new ImageUrl(Url.Create(path));
    }
}
