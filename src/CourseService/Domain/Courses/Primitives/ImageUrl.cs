namespace Courses.Domain.Courses.Primitives;

public record ImageUrl
{
    public string Path { get; init; }
    public ImageUrl(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Image path cannot be empty");

        Path = path;
    }

    public Uri GetFullUri(string baseDomain) => new Uri($"{baseDomain}/{Path}");
}
