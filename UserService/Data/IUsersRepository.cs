using UserService.Dtos;
using UserService.Models;

namespace UserService.Data
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> SearchUsersAsync(UserSearchDto Query);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserExistsAsync(int id);
        Task<int> CountUsersAsync(UserSearchDto query);
        Task<bool> SaveChangesAsync();
    }
}