using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<Permission>> GetPermissionsAsync(int roleId);
}