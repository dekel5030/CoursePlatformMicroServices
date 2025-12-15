using Gateway.Api.Models;

namespace Gateway.Api.Services.UserEnrichmentService;

public interface IUserEnrichmentService
{
    Task<UserEnrichmentModel> GetUserPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default);
}