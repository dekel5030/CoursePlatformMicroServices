using System.Text.Json;
using Auth.Application.Abstractions.Caching;
using CoursePlatform.ServiceDefaults.Messaging.Behaviors;
using Microsoft.Extensions.Caching.Distributed;

namespace Auth.Infrastructure.Redis;

internal class RedisCache : Application.Abstractions.Caching.ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken)
    {
        string? cachedValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(cachedValue))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<TResponse>(
        string cacheKey,
        TResponse result,
        TimeSpan? expiration,
        CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        byte[] serializedData = JsonSerializer.SerializeToUtf8Bytes(result, _serializerOptions);

        await _distributedCache.SetAsync(cacheKey, serializedData, options, cancellationToken);
    }
}
