using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Security;
using AuthService.SyncDataServices.Grpc;
using AuthService.SyncDataServices.Http;
using AutoMapper;
using Common;
using Common.Errors;
using Common.Rollback;
using Common.Utils;
using Microsoft.Extensions.Logging;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IUserServiceDataClient _usersClient;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRollbackManager _rollbackManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IGrpcUserServiceDataClient _grpcUsersClient;

    public AuthService(IAuthRepository repository,
                        IUserServiceDataClient usersClient,
                        IMapper mapper,
                        IPasswordHasher passwordHasher,
                        ITokenService tokenService,
                        IRollbackManager rollbackManager,
                        ILogger<AuthService> logger,
                        IGrpcUserServiceDataClient grpcUsersClient)
    {
        _repository = repository;
        _usersClient = usersClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _rollbackManager = rollbackManager;
        _logger = logger;
        _grpcUsersClient = grpcUsersClient;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        if (await IsUserCredentialsExistsAsync(registerRequestDto.Email))
        {
            return Result<AuthResponseDto>.Failure(Error.DuplicateEmail);
        }

        var userCreateDto = CreateUserCreateDto(registerRequestDto);
        var userServiceResult = await CreateUserInUserServiceAsync(userCreateDto);

        if (!userServiceResult.IsSuccess)
        {
            return Result<AuthResponseDto>.Failure(userServiceResult.Error!);
        }

        _rollbackManager.Add(() => RetryHelper.RetryAsync(() => _usersClient.DeleteUserAsync(userServiceResult.Value!.Id)));
        var userReadDto = userServiceResult.Value;
        var userCredentials = CreateUserCredentials(userReadDto!, registerRequestDto.Password);

        return await TrySaveCredentialsAndRespondAsync(userCredentials, registerRequestDto.Email);
    }

    public async Task<Result<AuthResponseDto>> RegisterAsyncGrpc(RegisterRequestDto registerRequestDto)
    {
        if (await IsUserCredentialsExistsAsync(registerRequestDto.Email))
        {
            return Result<AuthResponseDto>.Failure(Error.DuplicateEmail);
        }

        var userCreateDto = CreateUserCreateDto(registerRequestDto);
        var userServiceResult = await CreateUserInUserServiceAsyncGrpc(userCreateDto);

        if (!userServiceResult.IsSuccess)
        {
            return Result<AuthResponseDto>.Failure(userServiceResult.Error!);
        }

        _rollbackManager.Add(() => RetryHelper.RetryAsync(() => _usersClient.DeleteUserAsync(userServiceResult.Value!.Id)));
        var userReadDto = userServiceResult.Value;
        var userCredentials = CreateUserCredentials(userReadDto!, registerRequestDto.Password);

        return await TrySaveCredentialsAndRespondAsync(userCredentials, registerRequestDto.Email);
    }

    private async Task<Result<UserReadDto>> CreateUserInUserServiceAsyncGrpc(UserCreateDto userCreateDto)
    {
        var result = await RetryHelper.RetryAsync(() => _grpcUsersClient.CreateUserAsync(userCreateDto));

        if (result.IsSuccess)
        {
            _logger.LogInformation("UserService successfully created user for email {Email}", userCreateDto.Email);
        }
        else
        {
            _logger.LogError("UserService failed to create user for email {Email}: {Error}", userCreateDto.Email, result.Error);
        }

        return result;
    }

    private async Task SaveCredentialsAsync(UserCredentials credentials)
    {
        await RetryHelper.RetryAsync(() => _repository.AddUserCredentialsAsync(credentials));
        await RetryHelper.RetryAsync(() => _repository.SaveChangesAsync());
        _logger.LogInformation("User credentials for email {Email} saved successfully", credentials.Email);
    }

    private UserCreateDto CreateUserCreateDto(RegisterRequestDto registerRequestDto)
    {
        var userCreateDto = _mapper.Map<UserCreateDto>(registerRequestDto);
        userCreateDto.PasswordHash = _passwordHasher.Hash(registerRequestDto.Password);
        return userCreateDto;
    }

    private async Task<bool> IsUserCredentialsExistsAsync(string email)
    {
        var existingCredentials = await _repository.GetUserCredentialsByEmailAsync(email);
        return existingCredentials != null;
    }

    private UserCredentials CreateUserCredentials(UserReadDto userReadDto, string plainPassword)
    {
        var credentials = _mapper.Map<UserCredentials>(userReadDto);
        credentials.PasswordHash = _passwordHasher.Hash(plainPassword);

        return credentials;
    }

    private AuthResponseDto GenerateAuthResponseDto(UserCredentials userCredentials)
    {
        var authResponseDto = _mapper.Map<AuthResponseDto>(userCredentials);
        authResponseDto.Token = _tokenService.GenerateToken(_mapper.Map<TokenRequestDto>(userCredentials));

        return authResponseDto;
    }

    private async Task<Result<UserReadDto>> CreateUserInUserServiceAsync(UserCreateDto userCreateDto)
    {
        var result = await RetryHelper.RetryAsync(() => _usersClient.CreateUserAsync(userCreateDto));

        if (result.IsSuccess)
        {
            _logger.LogInformation("UserService successfully created user for email {Email}", userCreateDto.Email);
        }
        else
        {
            _logger.LogError("UserService failed to create user for email {Email}: {Error}", userCreateDto.Email, result.Error);
        }

        return result;
    }
    
    private async Task<Result<AuthResponseDto>> TrySaveCredentialsAndRespondAsync(UserCredentials credentials, string email)
    {
        try
        {
            await SaveCredentialsAsync(credentials);
            return Result<AuthResponseDto>.Success(GenerateAuthResponseDto(credentials));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving user credentials for email {Email}", email);
            await _rollbackManager.ExecuteAllAsync();
            return Result<AuthResponseDto>.Failure(Error.DatabaseError);
        }
    }

}