using Courses.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Courses.Infrastructure.Storage;

internal class StorageUrlResolver : IStorageUrlResolver
{
    private readonly StorageOptions _options;

    public StorageUrlResolver(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public Task<ResolvedUrl> ResolveAsync(StorageCategory category, string relativePath)
    {
        if (!_options.BucketMapping.TryGetValue(category, out var bucketName))
        {
            throw new InvalidOperationException($"No bucket mapped for category: {category}");
        }

        var resolvedUrl = new ResolvedUrl(
            Value: $"{_options.BaseUrl.TrimEnd('/')}/{bucketName}/{relativePath.TrimStart('/')}",
            Category: category
        );

        return Task.FromResult(resolvedUrl);
    }
}