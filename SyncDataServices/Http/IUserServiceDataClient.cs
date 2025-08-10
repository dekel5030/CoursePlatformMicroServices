using AuthService.Dtos;
using Common;

namespace AuthService.SyncDataServices.Http;

public interface IUserServiceDataClient
{
    Task<Result<UserServiceReadDto>> CreateUserAsync(UserCreateDto userCreateDto);
    Task<Result<UserServiceReadDto>> DeleteUserAsync(int id);
}