namespace Courses.Domain.Shared.Primitives;

public record Url
{
    public string Path { get; init; }

    #pragma warning disable CS8618
    protected Url() { }
    #pragma warning restore CS8618

    public Url(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be empty");
        }

        if (!Uri.IsWellFormedUriString(path, UriKind.Relative))
        {
            throw new ArgumentException("Path must be a valid relative URI (no domain allowed)");
        }

        Path = path.Trim().TrimStart('/');
    }

    public Uri ToUri(string baseDomain) => new Uri(new Uri(baseDomain), Path);

    public override string ToString() => Path;
}
