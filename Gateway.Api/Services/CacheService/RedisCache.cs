using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Gateway.Api.Services.CacheService;

internal sealed class RedisCache : ICacheService
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

    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
    {
        var cachedValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

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
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        var serializedData = JsonSerializer.SerializeToUtf8Bytes(result, _serializerOptions);

        await _distributedCache.SetAsync(cacheKey, serializedData, options, cancellationToken);
    }
}
