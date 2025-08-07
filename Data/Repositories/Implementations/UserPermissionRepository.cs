using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly AuthDbContext _dbContext;

    public UserPermissionRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }
   
    public async Task<IEnumerable<Permission>> GetPermissionsForUserAsync(int userId)
    {
        return await _dbContext.UserPermissions
            .Where(u => u.UserId == userId)
            .Select(up => up.Permission)
            .ToListAsync();
    }
}
