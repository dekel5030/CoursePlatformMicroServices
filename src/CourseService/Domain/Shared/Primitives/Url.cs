namespace Courses.Domain.Shared.Primitives;

public sealed record Url
{
    public string Path { get; }

    #pragma warning disable CS8618 
    private Url() { }
    #pragma warning restore CS8618

    private Url(string path)
    {
        Path = path;
    }

    public static Url Create(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("URL path cannot be empty");

        return new Url(path.Trim());
    }

    public Uri ToUri(string baseDomain)
    {
        if (string.IsNullOrWhiteSpace(baseDomain))
            throw new ArgumentException("Base domain cannot be empty");

        return new Uri($"{baseDomain.TrimEnd('/')}/{Path.TrimStart('/')}");
    }
}
