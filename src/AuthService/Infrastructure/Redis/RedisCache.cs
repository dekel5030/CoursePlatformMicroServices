using System.Text.Json;
using Application.Abstractions.Caching;
using Kernel;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Redis;

internal class RedisCache : ICacheService
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
        TResponse value, 
        TimeSpan? expiration, 
        CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        byte[] serializedData = JsonSerializer.SerializeToUtf8Bytes(value, _serializerOptions);

        await _distributedCache.SetAsync(cacheKey, serializedData, options, cancellationToken);
    }
}
