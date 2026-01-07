namespace Courses.Domain.Shared.Primitives;

public sealed record ImageUrl : Url
{
    private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    #pragma warning disable CS8618
    private ImageUrl() : base() { }
    #pragma warning restore CS8618

    public ImageUrl(string path) : base(path)
    {
        if (!_allowedExtensions.Any(ext => Path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Invalid image format");
    }
}
