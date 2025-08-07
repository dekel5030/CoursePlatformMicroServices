using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<Role?> GetByIdAsync(int id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task SaveChangesAsync();
}