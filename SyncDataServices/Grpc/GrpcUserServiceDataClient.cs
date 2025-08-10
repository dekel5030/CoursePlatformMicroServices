using AuthService.Dtos;
using AutoMapper;
using Common;
using Common.Grpc;
using static Common.Grpc.GrpcUserService;
using Error = Common.Errors.Error;

namespace AuthService.SyncDataServices.Grpc;

public class GrpcUserServiceDataClient : IGrpcUserServiceDataClient
{
    private readonly GrpcUserServiceClient _client;
    private readonly IMapper _mapper;
    private readonly ILogger<GrpcUserServiceDataClient> _logger;

    public GrpcUserServiceDataClient(GrpcUserServiceClient client, IMapper mapper, ILogger<GrpcUserServiceDataClient> logger)
    {
        _client = client;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserServiceReadDto>> CreateUserAsync(UserCreateDto userCreateDto)
    {
        var request = _mapper.Map<UserCreateRequest>(userCreateDto);

        var response = await _client.CreateUserAsync(request);
        var result = _mapper.Map<Result<UserServiceReadDto>>(response);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to create user in UserService. Email: {Email}, Error: {Error}", request.Email, result.Error);
        }
        else
        {
            _logger.LogInformation("User created successfully in UserService. ID: {Id}, Email: {Email}", result.Value!.Id, result.Value.Email);
        }

        return result;
    }

    public async Task<Result<bool>> DeleteUserAsync(int id)
    {
        var request = new UserDeleteRequest { Id = id };

        var response = await _client.DeleteUserAsync(request);
        var result = _mapper.Map<Result<bool>>(response);

        if (!result.IsSuccess)
        {
            var error = _mapper.Map<Error>(result.Error);
            _logger.LogError("Failed to delete user in UserService. ID: {Id}, Error: {Error}", id, error);
        }
        else
        {
            _logger.LogInformation("User with ID {Id} deleted successfully in UserService", id);
        }

        return result;
    }
}