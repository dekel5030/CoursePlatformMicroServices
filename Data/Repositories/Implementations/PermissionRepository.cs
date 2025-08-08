using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Dtos.Permissions;
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

    public async Task AddPermissionAsync(Permission permission)
    {
        await _dbContext.Permissions.AddAsync(permission);
    }

    public void DeletePermission(Permission permission)
    {
        _dbContext.Permissions.Remove(permission);
    }

    public async Task<Permission?> GetPermissionByIdAsync(int id)
    {
        return await _dbContext.Permissions.FindAsync(id);
    }

    public async Task<IEnumerable<Permission>> GetPermissionsAsync(PermissionSearchDto queryDto)
    {
        var query = _dbContext.Permissions.AsQueryable();

        if (!string.IsNullOrEmpty(queryDto.Name))
        {
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{queryDto.Name}%"));
        }

        return await query.ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
