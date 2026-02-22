using Courses.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Courses.Infrastructure.Storage;

internal sealed class StorageBucketResolver : IStorageBucketResolver
{
    private readonly StorageOptions _options;

    public StorageBucketResolver(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public string GetBucket(StorageCategory category)
    {
        if (!_options.BucketMapping.TryGetValue(category, out string? bucketName))
        {
            throw new InvalidOperationException($"No bucket mapped for category: {category}");
        }

        return bucketName;
    }
}
