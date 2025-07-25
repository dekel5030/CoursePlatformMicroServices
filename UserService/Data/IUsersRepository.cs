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
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);

        Task ActivateUserAsync(int userId);
        Task DeactivateUserAsync(int userId);

        Task<bool> IsEmailConfirmedAsync(int userId);
        Task<bool> SaveChangesAsync();
    }
}