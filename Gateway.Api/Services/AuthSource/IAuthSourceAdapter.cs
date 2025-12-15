using Gateway.Api.Models;

namespace Gateway.Api.Services.AuthSource;

public interface IAuthSourceAdapter
{
    Task<UserEnrichmentModel?> FetchPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default);
}