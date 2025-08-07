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

    public async Task AddAsync(Role role)
    {
       await _dbContext.Roles.AddAsync(role);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _dbContext.Roles.ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _dbContext.Roles.FindAsync(id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
