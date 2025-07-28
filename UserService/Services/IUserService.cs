using UserService.Common;
using UserService.Dtos;

namespace UserService.Services
{
    public interface IUserService
    {
        // === Create & Add ===
        Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);

        // === Read ===
        Task<UserReadDto?> GetUserByIdAsync(int userId);
        Task<UserReadDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserReadDto>> GetPagedUsersAsync(int pageNumber, int pageSize);
        Task<IEnumerable<UserReadDto>> SearchUsersAsync(string query);

        // === Update ===
        Task<Result<UserReadDto>> SetUserActivationAsync(int userId, bool isActive);

        // === Delete ===
        Task<Result<UserReadDto>> DeleteUserAsync(int userId);

        // === Validation ===
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> IsEmailConfirmedAsync(int userId);
    }
}