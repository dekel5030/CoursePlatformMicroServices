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
   
    public async Task<IEnumerable<Permission>> GetPermissionsForRoleAsync(int roleId)
    {
        return await _dbContext.RolePermissions
            .Where(u => u.RoleId == roleId)
            .Select(up => up.Permission)
            .ToListAsync();
    }
}
