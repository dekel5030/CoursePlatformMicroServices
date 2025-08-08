using AuthService.Data.Context;
using AuthService.Data.Repositories.Interfaces;
using AuthService.Models;
using Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Implementations;

public class UserCredentialsRepository : IUserCredentialsRepository
{
    private readonly AuthDbContext _dbContext;

    public UserCredentialsRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserCredentialsAsync(UserCredentials credentials)
    {
        await _dbContext.UserCredentials.AddAsync(credentials);
    }

    public void DeleteUserCredentialsAsync(UserCredentials credentials)
    {
        _dbContext.UserCredentials.Remove(credentials);
    }

    public async Task<UserCredentials?> GetUserCredentialsByEmailAsync(string email, bool includeAccessData = false)
    {
        var query = _dbContext.UserCredentials.AsQueryable();

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