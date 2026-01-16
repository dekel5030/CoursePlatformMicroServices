namespace Courses.Domain.Shared.Primitives;

public sealed record VideoUrl : Url
{
    private static readonly string[] _allowedExtensions = [".mp4", ".webm", ".mov"];

#pragma warning disable CS8618
    private VideoUrl() : base() { }
#pragma warning restore CS8618

    public VideoUrl(string path) : base(path)
    {
        if (!_allowedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Invalid video format");
        }
    }
}
