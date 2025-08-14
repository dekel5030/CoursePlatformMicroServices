using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly AuthDbContext _dbContext;

    public PermissionRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Permission?> GetPermissionByIdAsync(int id)
    {
        return await _dbContext.Permissions.FindAsync(id);
    }

    public async Task<Permission?> GetPermissionByNameAsync(string name)
    {
        return await _dbContext.Permissions
            .FirstOrDefaultAsync(p => EF.Functions.ILike(p.Name, name));
    }

    public async Task<(IEnumerable<Permission>, int TotalCount)> GetAllPermissionsAsync()
    {
        var permissions = await _dbContext.Permissions.ToListAsync();
        
        return (permissions, permissions.Count);
    }

    public async Task AddPermissionAsync(Permission permission)
    {
        await _dbContext.Permissions.AddAsync(permission);
    }

    public void DeletePermission(Permission permission)
    {
        _dbContext.Permissions.Remove(permission);
    }

    public Task<bool> ExistsByIdAsync(int permissionId)
    {
        return _dbContext.Permissions.AnyAsync(p => p.Id == permissionId);
    }

    public Task<bool> ExistsByNameAsync(string permissionName)
    {
        return _dbContext.Permissions.AnyAsync(p => p.Name == permissionName);
    }
}
