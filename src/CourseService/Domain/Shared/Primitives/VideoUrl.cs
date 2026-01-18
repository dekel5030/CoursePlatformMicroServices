using Kernel;

namespace Courses.Domain.Shared.Primitives;

public sealed record VideoUrl : Url
{
    private static readonly string[] _allowedExtensions = [".mp4", ".webm", ".mov"];

#pragma warning disable CS8618
    private VideoUrl() : base() { }
#pragma warning restore CS8618

    public VideoUrl(string path) : base(path)
    {
        string extension = System.IO.Path.GetExtension(path).ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Invalid video format");
        }
    }

    public static Result<VideoUrl> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<VideoUrl>(Error.Validation("VideoUrl.Empty", "Video URL cannot be empty."));
        }

        string extension = System.IO.Path.GetExtension(value).ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension))
        {
            return Result.Failure<VideoUrl>(Error.Validation("Upload.InvalidFormat", "Invalid video format. Allowed formats: mp4, webm, mov."));
        }

        return Result.Success(new VideoUrl(value));
    }
}
