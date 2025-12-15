using Gateway.Api.Models;
using Gateway.Api.Services.AuthCacheRepository;
using Gateway.Api.Services.AuthSource;

namespace Gateway.Api.Services.UserEnrichmentService;

public class UserEnrichmentService : IUserEnrichmentService
{
    private readonly IAuthCacheRepository _cacheRepo;
    private readonly IAuthSourceAdapter _sourceAdapter;
    private readonly ILogger<UserEnrichmentService> _logger;

    public UserEnrichmentService(
        IAuthCacheRepository cache,
        IAuthSourceAdapter source,
        ILogger<UserEnrichmentService> logger)
    {
        _cacheRepo = cache;
        _sourceAdapter = source;
        _logger = logger;
    }

    public async Task<UserEnrichmentModel> GetUserPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _cacheRepo.GetAsync(userId, cancellationToken);
            if (cached != null)
            {
                _logger.LogDebug("Cache hit for user {UserId}", userId);
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache unavailable, falling back to source");
        }

        _logger.LogDebug("Cache miss for user {UserId}, fetching from source", userId);
        UserEnrichmentModel? freshData = await _sourceAdapter
            .FetchPermissionsAsync(userId, cancellationToken);

        if (freshData == null)
        {
            _logger.LogWarning("No permissions found for user {UserId} from source", userId);
            return new UserEnrichmentModel
            {
                UserId = userId,
                Permissions = [],
                Roles = []
            };
        }

        return new UserEnrichmentModel
        {
            UserId = freshData.UserId,
            Permissions = freshData.Permissions,
            Roles = freshData.Roles
        };
    }
}