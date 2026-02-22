using Courses.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Courses.Infrastructure.Storage;

internal sealed class StorageService : IStorageUrlResolver, IObjectStorageService
{
    private readonly StorageOptions _options;

    public StorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public PresignedUrlResponse GenerateUploadUrlAsync(
        StorageCategory category,
        string fileKey,
        string referenceId,
        string referenceType,
        TimeSpan? expiry = null,
        Dictionary<string, string>? metadata = null)
    {
        if (!_options.BucketMapping.TryGetValue(category, out string? bucketName))
        {
            throw new InvalidOperationException($"No bucket mapped for category: {category}");
        }

        string serviceName = "courseservice";
        string baseUrl = _options.BaseUrl.TrimEnd('/');

        string url = $"{baseUrl}/upload/{bucketName}/{fileKey.TrimStart('/')}";

        var headers = new Dictionary<string, string>
        {
            { "x-owner-service", serviceName },
            { "x-ref-id", referenceId },
            { "x-ref-type", referenceType }
        };

        if (metadata != null)
        {
            foreach (KeyValuePair<string, string> item in metadata)
            {
                headers[$"x-meta-{item.Key.ToLowerInvariant()}"] = item.Value;
            }
        }

        return new PresignedUrlResponse(
            Url: url,
            FileKey: fileKey,
            ExpiresAt: DateTime.UtcNow.Add(expiry ?? TimeSpan.FromMinutes(15)),
            Headers: headers
        );
    }

    public PresignedUrlResponse GenerateViewUrl(string fileKey, TimeSpan expiry)
    {
        throw new NotImplementedException();
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
