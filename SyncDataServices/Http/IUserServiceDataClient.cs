using AuthService.Dtos;
using Common;

namespace AuthService.SyncDataServices.Http;

public interface IUserServiceDataClient
{
    Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);
    Task<Result<UserReadDto>> DeleteUserAsync(int id);
}