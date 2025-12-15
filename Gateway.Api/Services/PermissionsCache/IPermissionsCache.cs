using Gateway.Api.Models;

namespace Gateway.Api.Services.PermissionsCache;

public interface IPermissionsCache
{
    Task<UserPermissionsDto?> GetAsync(
        string userId, 
        CancellationToken cancellationToken = default);
    
    Task SetAsync(
        string userId, 
        UserPermissionsDto permissions, 
        CancellationToken cancellationToken = default);
}