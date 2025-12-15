using Gateway.Api.Models;

namespace Gateway.Api.Services.AuthCacheRepository;

public interface IAuthCacheRepository
{
    Task<UserEnrichmentModel?> GetAsync(
        string userId, 
        CancellationToken cancellationToken = default);
}