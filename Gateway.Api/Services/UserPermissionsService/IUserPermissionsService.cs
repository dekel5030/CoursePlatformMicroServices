using Gateway.Api.Models;

namespace Gateway.Api.Services.UserPermissionsService;

public interface IUserPermissionsService
{
    Task<UserPermissionsDto> GetUserPermissionsAsync(
        string userId, 
        CancellationToken cancellationToken = default);
}