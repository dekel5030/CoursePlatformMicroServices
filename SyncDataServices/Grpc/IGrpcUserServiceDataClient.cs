using AuthService.Dtos;
using Common;

namespace AuthService.SyncDataServices.Grpc;

public interface IGrpcUserServiceDataClient
{
    Task<Result<UserServiceReadDto>> CreateUserAsync(UserCreateDto userCreateDto);
    Task<Result<bool>> DeleteUserAsync(int id);
}
