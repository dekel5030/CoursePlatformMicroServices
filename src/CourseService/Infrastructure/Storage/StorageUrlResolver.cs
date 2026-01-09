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

    public Task<ResolvedUrl> ResolveAsync(
        StorageCategory category, 
        string relativePath, 
        CancellationToken cancellationToken = default)
    {
        if (!_options.BucketMapping.TryGetValue(category, out var bucketName))
        {
            throw new InvalidOperationException($"No bucket mapped for category: {category}");
        }

        var urlString = $"{_options.BaseUrl.TrimEnd('/')}/{bucketName}/{relativePath.TrimStart('/')}";
        var uri = new Uri(urlString);

        var resolvedUrl = new ResolvedUrl(
            Value: uri,
            Category: category
        );

        return Task.FromResult(resolvedUrl);
    }
}