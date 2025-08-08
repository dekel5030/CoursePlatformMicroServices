using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IRolePermissionRepository
{
    Task<IEnumerable<Permission>> GetPermissionsAsync(int roleId);
    Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds);
    Task RemovePermissionAsync(int roleId, int permissionId);
    Task<bool> SaveChangesAsync();
}