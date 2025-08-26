using AutoMapper;
using Common.Grpc;
using Grpc.Core;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;
using static Common.Grpc.GrpcUserService;

namespace UserService.SyncDataServices.Grpc;

public class GrpcUserService : GrpcUserServiceBase
{
    private readonly ILogger<GrpcUserService> _logger;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public GrpcUserService(IUserService userService, IMapper mapper, ILogger<GrpcUserService> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task<Result_UserReadResponse> CreateUser(UserCreateRequest request, ServerCallContext context)
    {
        var userCreateDto = _mapper.Map<UserCreateDto>(request);
        var result = await _userService.CreateUserAsync(userCreateDto);

        if (!result.IsSuccess)
        {
            _logger.LogError("User creation failed. Error: {error}, Email: {email}", result.Error, request.Email);
        }
        else
        {
            _logger.LogInformation("User id {id} creation succeeded.", result.Value!.Id);
        }

        return _mapper.Map<Result_UserReadResponse>(result);
    }

    public override async Task<Result_Bool> DeleteUser(UserDeleteRequest request, ServerCallContext context)
    {
        var result = await _userService.DeleteUserAsync(request.Id);

        if (!result.IsSuccess)
        {
            _logger.LogError("User deletion failed. Error: {error}, Id: {id}", result.Error, request.Id);
        }
        else
        {
            _logger.LogInformation("User id {id} deletion succeeded.", request.Id);
        }

        return _mapper.Map<Result_Bool>(result);
    }
}