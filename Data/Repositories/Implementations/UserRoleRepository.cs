using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AuthDbContext _dbContext;

    public UserRoleRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AssignRoleAsync(int userId, int roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await _dbContext.UserRoles.AddAsync(userRole);
    }

    public async Task<IEnumerable<Role>> GetRolesForUserAsync(int userId)
    {
        return await _dbContext.UserRoles
            .Where(u => u.UserId == userId)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<bool> HasRoleAsync(int userId, int roleId)
    {
        return await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
