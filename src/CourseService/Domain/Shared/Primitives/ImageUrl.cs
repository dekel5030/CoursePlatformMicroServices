using Kernel;

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
        {
            throw new ArgumentException("Invalid image format");
        }
    }

    public static Result<ImageUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ImageUrl>(Error.Validation("ImageUrl.Empty", "Image URL cannot be empty."));
        }

        string extension = System.IO.Path.GetExtension(value).ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension))
        {
            return Result.Failure<ImageUrl>(Error.Validation("Upload.InvalidFormat", "Invalid image format. Allowed formats: jpg, jpeg, png, webp."));
        }

        return Result.Success(new ImageUrl(value));
    }
}
