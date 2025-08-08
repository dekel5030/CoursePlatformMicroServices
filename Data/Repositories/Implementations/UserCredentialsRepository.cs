using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class AuthUserRepository : IAuthUserRepository
{
    private readonly AuthDbContext _dbContext;

    public AuthUserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAuthUserAsync(AuthUser authUser)
    {
        await _dbContext.AuthUser.AddAsync(authUser);
    }

    public void DeleteAuthUserAsync(AuthUser authUser)
    {
        _dbContext.AuthUser.Remove(authUser);
    }

    public async Task<AuthUser?> GetAuthUserByEmailAsync(string email, bool includeAccessData = false)
    {
        var query = _dbContext.AuthUser.AsQueryable();

        if (includeAccessData)
        {
            query = query
                .Include(x => x.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)

                .Include(x => x.UserPermissions)
                    .ThenInclude(up => up.Permission);
        }

        return await query.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}