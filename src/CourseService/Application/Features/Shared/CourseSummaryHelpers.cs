using Courses.Application.Abstractions.Storage;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Features.Shared;

public static class CourseSummaryHelpers
{
    public const int DefaultShortDescriptionMaxLength = 100;

    public static string TruncateShortDescription(string value, int maxLength = DefaultShortDescriptionMaxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length > maxLength ? value[..maxLength] + "..." : value;
    }

    public static string? GetFirstImagePublicUrl(
        IReadOnlyCollection<ImageUrl> images,
        IStorageUrlResolver urlResolver)
    {
        if (images == null || images.Count == 0)
        {
            return null;
        }

        ImageUrl? first = images.FirstOrDefault();
        return first is null ? null : urlResolver.ResolvePublicUrl(first.Path);
    }
}
