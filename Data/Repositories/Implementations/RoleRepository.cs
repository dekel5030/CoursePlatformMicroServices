using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _dbContext;

    public RoleRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await _dbContext.Roles.FindAsync(id);
    }

    public async Task<Role?> GetRoleByNameAsync(string name)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _dbContext.Roles.ToListAsync();
    }

    public Task<Role?> GetRoleWithAccessDataByIdAsync(int id)
    {
        return _dbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<Role?> GetRoleWithAccessDataByNameAsync(string name)
    {
        return _dbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
    }

    public async Task AddRoleAsync(Role role)
    {
        await _dbContext.Roles.AddAsync(role);
    }

    public Task RemoveRoleAsync(Role role)
    {
        _dbContext.Roles.Remove(role);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<Permission> Permissions, int TotalCount)> GetRolePermissionsAsync(int roleId)
    {
        var permissions = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .ToListAsync();

        var TotalCount = permissions.Count();

        return (permissions, TotalCount);
    }

    public async Task AddPermissionsAsync(params RolePermission[] rolePermissions)
    {
        await _dbContext.RolePermissions.AddRangeAsync(rolePermissions);
    }

    public Task RemovePermissionAsync(RolePermission rolePermission)
    {
        _dbContext.RolePermissions.Remove(rolePermission);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByIdAsync(int roleId)
    {
        return _dbContext.Roles.AnyAsync(r => r.Id == roleId);
    }

    public Task<bool> ExistsByNameAsync(string roleName)
    {
        return _dbContext.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower());
    }

    public Task<bool> HasPermission(int roleId, int permissionId)
    {
        return _dbContext.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }
}
