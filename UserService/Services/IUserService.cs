using UserService.Common;
using UserService.Dtos;

namespace UserService.Services
{
    public interface IUserService
    {
        // === Create & Add ===
        Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);

        // === Read ===
        Task<UserReadDto?> GetUserByIdAsync(int id);
        Task<UserReadDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<IEnumerable<UserReadDto>> SearchUsersAsync(string query);

        // === Update ===
        Task ActivateUserAsync(int userId);
        Task DeactivateUserAsync(int userId);

        // === Delete ===
        Task DeleteUserAsync(int id);

        // === Validation ===
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserExistsAsync(int id);
        Task<bool> IsEmailConfirmedAsync(int userId);
    }
}