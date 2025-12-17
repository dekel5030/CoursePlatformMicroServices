using System.Text.Json;
using Auth.Contracts.Dtos;
using Gateway.Api.Models;
using Kernel.Auth;
using Microsoft.Extensions.Caching.Distributed;

namespace Gateway.Api.Services.AuthCacheRepository;

public class AuthRedisRepository : IAuthCacheRepository
{
    private readonly IDistributedCache _cache;

    public AuthRedisRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<UserEnrichmentModel?> GetAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var key = string.Format(AuthCacheKeys.UserAuthDataPattern, userId);
        var json = await _cache.GetStringAsync(key, token: cancellationToken);

        if (string.IsNullOrEmpty(json)) return null;

        //await _cache.RefreshAsync(key, token: cancellationToken);

        var contract = JsonSerializer.Deserialize<UserAuthDataDto>(json);
        if (contract == null) return null;

        return new UserEnrichmentModel
        {
            UserId = contract.UserId,
            Permissions = contract.Permissions,
            Roles = contract.Roles
        };
    }
}