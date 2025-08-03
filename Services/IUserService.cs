using Common;
using UserService.Dtos;

namespace UserService.Services
{
    public interface IUserService
    {
        // === Create ===
        Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);

        // === Read ===
        Task<UserDetailsDto?> GetUserByIdAsync(int userId);
        Task<PagedResponseDto<UserReadDto>> GetUsersByQueryAsync(UserSearchDto query);

        // === Update ===
        Task<Result<UserReadDto>> SetUserActivationAsync(int userId, bool isActive);
        Task<Result<UserReadDto>> PatchUser(UserPatchDto userPatchDto);

        // === Delete ===
        Task<Result<UserReadDto>> DeleteUserAsync(int userId);
    }
}