using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IUserPermissionRepository
{
    Task<IEnumerable<Permission>> GetPermissionsAsync(int userId);
}