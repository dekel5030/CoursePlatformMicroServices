using Gateway.Api.Models;
using Gateway.Api.Services.PermissionsCache;
using Gateway.Api.Services.PermissionsSource;

namespace Gateway.Api.Services.UserPermissionsService;

public class UserPermissionsService : IUserPermissionsService
{
    private readonly IPermissionsCache _cache;
    private readonly IPermissionsSource _source;
    private readonly ILogger<UserPermissionsService> _logger;

    public UserPermissionsService(
        IPermissionsCache cache,
        IPermissionsSource source,
        ILogger<UserPermissionsService> logger)
    {
        _cache = cache;
        _source = source;
        _logger = logger;
    }

    public async Task<UserPermissionsDto> GetUserPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _cache.GetAsync(userId, cancellationToken);
            if (cached != null) return cached;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache unavailable, falling back to source");
        }

        var freshData = await _source.FetchPermissionsAsync(userId, cancellationToken);

        if (freshData != null)
        {
            try
            {
                await _cache.SetAsync(userId, freshData, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cache");
            }
        }

        return freshData ?? new UserPermissionsDto();
    }
}