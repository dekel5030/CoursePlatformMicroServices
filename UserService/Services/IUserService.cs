using UserService.Common;
using UserService.Dtos;

namespace UserService.Services
{
    public interface IUserService
    {
        // === Create ===
        Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);

        // === Read ===
        Task<UserReadDto?> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserDetailsDto>> GetUsersByQueryAsync(UserSearchDto query);

        // === Update ===
        Task<Result<UserReadDto>> SetUserActivationAsync(int userId, bool isActive);

        // === Delete ===
        Task<Result<UserReadDto>> DeleteUserAsync(int userId);
    }
}