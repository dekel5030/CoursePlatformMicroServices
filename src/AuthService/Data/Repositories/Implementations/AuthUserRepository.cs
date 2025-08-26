using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Dtos.AuthUsers;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class AuthUserRepository : IAuthUserRepository
{
    private readonly AuthDbContext _dbContext;

    public AuthUserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthUser?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.AuthUser.FindAsync(userId);
    }

    public async Task<AuthUser?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.AuthUser.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<(IEnumerable<AuthUser> users, int totalCount)> SearchUsersAsync(UserSearchDto query)
    {
        var userQuery = _dbContext.AuthUser.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            userQuery = userQuery.Where(u => u.Email.Contains(query.Email));
        }

        var totalCount = await userQuery.CountAsync();

        var users = await userQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public Task<AuthUser?> GetUserWithAccessDataByIdAsync(int userId)
    {
        return _dbContext.AuthUser
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserPermissions)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public Task<AuthUser?> GetUserWithAccessDataByEmailAsync(string email)
    {
        return _dbContext.AuthUser
            .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rolePermission => rolePermission.Permission)
            .Include(user => user.UserPermissions)
                .ThenInclude(userPermission => userPermission.Permission)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddUserAsync(AuthUser authUser)
    {
        await _dbContext.AuthUser.AddAsync(authUser);
    }

    public Task RemoveUserAsync(AuthUser authUser)
    {
        _dbContext.AuthUser.Remove(authUser);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<Permission> permissions, int totalCount)> GetUserPermissionsAsync(int userId)
    {
        var permissions = await _dbContext.UserPermissions
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission)
            .ToListAsync();

        var totalCount = permissions.Count;

        return (permissions, totalCount);
    }

    public async Task<(IEnumerable<Role> roles, int totalCount)> GetUserRolesAsync(int userId)
    {
        var roles = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .ToListAsync();

        var totalCount = roles.Count;

        return (roles, totalCount);
    }

    public async Task AssignRoleAsync(UserRole role)
    {
        await _dbContext.UserRoles.AddAsync(role);
    }

    public Task UnassignRoleAsync(UserRole role)
    {
        _dbContext.UserRoles.Remove(role);
        return Task.CompletedTask;
    }

    public async Task AddPermissionAsync(UserPermission userPermission)
    {
        await _dbContext.UserPermissions.AddAsync(userPermission);
    }

    public Task RemovePermissionAsync(UserPermission userPermission)
    {
        _dbContext.UserPermissions.Remove(userPermission);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByIdAsync(int userId)
    {
        return await _dbContext.AuthUser.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.AuthUser.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> HasPermissionAsync(int userId, int permissionId)
    {
        return await _dbContext.UserPermissions.AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);
    }

    public async Task<bool> HasRoleAsync(int userId, int roleId)
    {
        return await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }
}