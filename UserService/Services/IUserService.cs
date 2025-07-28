using UserService.Common;
using UserService.Dtos;

namespace UserService.Services
{
    public interface IUserService
    {
        // === Create & Add ===
        Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);

        // === Read ===
        Task<Result<UserReadDto>> GetUserByIdAsync(int id);
        Task<Result<UserReadDto>> GetUserByEmailAsync(string email);
        Task<Result<IEnumerable<UserReadDto>>> GetAllUsersAsync();
        Task<Result<IEnumerable<UserReadDto>>> SearchUsersAsync(string query);

        // === Update ===
        Task<Result<UserReadDto>> ActivateUserAsync(int userId);
        Task<Result<UserReadDto>> DeactivateUserAsync(int userId);

        // === Delete ===
        Task<Result<UserReadDto>> DeleteUserAsync(int id);

        // === Validation ===
        Task<Result<UserReadDto>> EmailExistsAsync(string email);
        Task<Result<UserReadDto>> UserExistsAsync(int id);
        Task<Result<UserReadDto>> IsEmailConfirmedAsync(int userId);
    }
}