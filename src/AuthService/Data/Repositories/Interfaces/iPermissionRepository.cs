using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetPermissionByIdAsync(int id);
    Task<Permission?> GetPermissionByNameAsync(string name);
    Task<(IEnumerable<Permission>, int TotalCount)> GetAllPermissionsAsync();

    Task AddPermissionAsync(Permission permission);
    void DeletePermission(Permission permission);

    Task<bool> ExistsByIdAsync(int permissionId);
    Task<bool> ExistsByNameAsync(string permissionName);
}