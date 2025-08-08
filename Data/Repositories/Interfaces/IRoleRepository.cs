using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string name);
    Task<Role?> GetRoleByIdAsync(int id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task AddAsync(Role role);
    void Remove(Role role);
    Task SaveChangesAsync();
}