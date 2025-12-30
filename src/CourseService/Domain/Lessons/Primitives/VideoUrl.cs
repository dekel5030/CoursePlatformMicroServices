using Courses.Domain.Shared.Primitives;

namespace Courses.Domain.Lessons.Primitives;

public sealed record VideoUrl
{
    private static readonly string[] _allowedExtensions = [".mp4", ".webm", ".mov"];

    public Url Value { get; }

    public string Path => Value.Path;

    private VideoUrl(Url value)
    {
        Value = value;
    }

    public static VideoUrl Create(string path)
    {
        if (!_allowedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Invalid video format");

        return new VideoUrl(Url.Create(path));
    }
}
