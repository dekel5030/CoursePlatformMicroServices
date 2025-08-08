using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly AuthDbContext _dbContext;

    public RolePermissionRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Permission>> GetPermissionsAsync(int roleId)
    {
        return await _dbContext.RolePermissions
            .Where(u => u.RoleId == roleId)
            .Select(up => up.Permission)
            .ToListAsync();
    }

    public async Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var rolePermissions = permissionIds.Select(permissionId => new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        });

        await _dbContext.RolePermissions.AddRangeAsync(rolePermissions);
    }

    public async Task RemovePermissionAsync(int roleId, int permissionId)
    {
        var rolePermission = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

        if (rolePermission != null)
        {
            _dbContext.RolePermissions.Remove(rolePermission);
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
