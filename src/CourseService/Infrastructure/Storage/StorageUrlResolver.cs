using Courses.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Courses.Infrastructure.Storage;

internal sealed class StorageUrlResolver : IStorageUrlResolver
{
    private readonly StorageOptions _options;

    public StorageUrlResolver(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public ResolvedUrl Resolve(
        StorageCategory category,
        string relativePath)
    {
        if (!_options.BucketMapping.TryGetValue(category, out string? bucketName))
        {
            throw new InvalidOperationException($"No bucket mapped for category: {category}");
        }

        string urlString = $"{_options.BaseUrl.TrimEnd('/')}/{bucketName}/{relativePath.TrimStart('/')}";
        var uri = new Uri(urlString);

        var resolvedUrl = new ResolvedUrl(
            Value: uri.ToString(),
            Category: category
        );

        return resolvedUrl;
    }
}
