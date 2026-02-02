using System;
using System.Collections.Generic;
using System.Text;
using CoursePlatform.ServiceDefaults.Messaging.Behaviors;
using Microsoft.Extensions.Caching.Memory;

namespace Courses.Infrastructure.Cache;

internal sealed class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken)
    {
        T? result = _memoryCache.Get<T>(cacheKey);
        return Task.FromResult(result);
    }

    public Task SetAsync<TResponse>(
        string cacheKey,
        TResponse result,
        TimeSpan? expiration,
        CancellationToken cancellationToken)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };

        _memoryCache.Set(cacheKey, result, cacheOptions);

        return Task.CompletedTask;
    }
}
