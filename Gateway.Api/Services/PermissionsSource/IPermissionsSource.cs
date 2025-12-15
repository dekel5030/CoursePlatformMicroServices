using Gateway.Api.Models;

namespace Gateway.Api.Services.PermissionsSource;

public interface IPermissionsSource
{
    Task<UserPermissionsDto?> FetchPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default);
}