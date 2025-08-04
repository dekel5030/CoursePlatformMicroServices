using AuthService.Dtos;
using Common;

namespace AuthService.SyncDataServices.Grpc;

public interface IGrpcUserServiceDataClient
{
    Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto);
    Task<Result<bool>> DeleteUserAsync(int id);
}
