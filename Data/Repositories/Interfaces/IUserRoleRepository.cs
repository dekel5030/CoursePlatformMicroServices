using AuthService.Models;

namespace AuthService.Data.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task AssignRoleAsync(int userId, string roleName);
    Task AssignRoleAsync(int userId, int roleId);
    Task<bool> HasRoleAsync(int userId, int roleId);
    Task<IEnumerable<Role>> GetRolesForUserAsync(int userId);
    Task<bool> SaveChangesAsync();
}
