using AuthService.Dtos.Permissions;
using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetPermissionByIdAsync(int id);
    Task<Permission?> GetPermissionByNameAsync(string name);
    Task<IEnumerable<Permission>> GetPermissionsAsync(PermissionSearchDto queryDto);
    Task AddPermissionAsync(Permission permission);
    void DeletePermission(Permission permission);
    Task<bool> SaveChangesAsync();
}