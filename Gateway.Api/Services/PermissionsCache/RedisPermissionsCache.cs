using System.Text.Json;
using Gateway.Api.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Gateway.Api.Services.PermissionsCache;

public class RedisPermissionsCache : IPermissionsCache
{
    private readonly IDistributedCache _cache;
    private const string KeyPrefix = "gateway:permissions:";
    private const int ExpirationMinutes = 15;

    public RedisPermissionsCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<UserPermissionsDto?> GetAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var key = $"{KeyPrefix}{userId}";
        var json = await _cache.GetStringAsync(key, token: cancellationToken);

        if (string.IsNullOrEmpty(json)) return null;

        await _cache.RefreshAsync(key, token: cancellationToken);

        return JsonSerializer.Deserialize<UserPermissionsDto>(json);
    }

    public async Task SetAsync(
        string userId, 
        UserPermissionsDto permissions, 
        CancellationToken cancellationToken = default)
    {
        var key = $"{KeyPrefix}{userId}";
        var json = JsonSerializer.Serialize(permissions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ExpirationMinutes)
        };

        await _cache.SetStringAsync(key, json, options, token: cancellationToken);
    }
}