using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetRoleByIdAsync(int roleId);
    Task<Role?> GetRoleByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();

    Task<Role?> GetRoleWithAccessDataByIdAsync(int roleId);
    Task<Role?> GetRoleWithAccessDataByNameAsync(string name);

    Task AddRoleAsync(Role role);
    Task RemoveRoleAsync(Role role);

    Task<(IEnumerable<Permission> Permissions, int TotalCount)> GetRolePermissionsAsync(int roleId);

    Task AddPermissionsAsync(params RolePermission[] rolePermissions);
    Task RemovePermissionAsync(RolePermission rolePermission);

    Task<bool> ExistsByIdAsync(int roleId);
    Task<bool> ExistsByNameAsync(string roleName);
    Task<bool> HasPermission(int roleId, int permissionId);
}