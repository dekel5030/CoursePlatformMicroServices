using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> SearchUsersAsync(string query);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}