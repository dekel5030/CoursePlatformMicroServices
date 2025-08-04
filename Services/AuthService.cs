using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Security;
using AuthService.SyncDataServices.Grpc;
using AutoMapper;
using Common;
using Common.Errors;
using Common.Rollback;
using Common.Utils;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRollbackManager _rollbackManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IGrpcUserServiceDataClient _usersClient;

    public AuthService(IAuthRepository repository,
                        IMapper mapper,
                        IPasswordHasher passwordHasher,
                        ITokenService tokenService,
                        IRollbackManager rollbackManager,
                        ILogger<AuthService> logger,
                        IGrpcUserServiceDataClient usersClient)
    {
        _repository = repository;
        _usersClient = usersClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _rollbackManager = rollbackManager;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var normalizedEmail = registerRequestDto.Email.ToLowerInvariant();
        
        if (await _repository.GetUserCredentialsByEmailAsync(normalizedEmail) != null)
        {
            _logger.LogInformation("Registration attempt with existing email: {Email}", normalizedEmail);
            return Result<AuthResponseDto>.Failure(Error.DuplicateEmail);
        }

        var userCreateDto = _mapper.Map<UserCreateDto>(registerRequestDto);
        userCreateDto.Email = normalizedEmail;
        userCreateDto.PasswordHash = _passwordHasher.Hash(registerRequestDto.Password);

        var userServiceResult = await CreateUserInUserServiceAsync(userCreateDto);
        if (!userServiceResult.IsSuccess)
        {
            return Result<AuthResponseDto>.Failure(userServiceResult.Error!);
        }

        _rollbackManager.Add(() => RetryHelper.RetryAsync(() => _usersClient.DeleteUserAsync(userServiceResult.Value!.Id)));
        var userReadDto = userServiceResult.Value;
        var userCredentials = CreateUserCredentials(userReadDto!, userCreateDto.PasswordHash);

        return await TrySaveCredentialsAndRespondAsync(userCredentials, registerRequestDto.Email);
    }

    private UserCredentials CreateUserCredentials(UserReadDto userReadDto, string hashedPassword)
    {
        var credentials = _mapper.Map<UserCredentials>(userReadDto);
        credentials.PasswordHash = hashedPassword; // השתמש בסיסמה המוצפנת כפי שהיא

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
        var result = await RetryHelper
                            .RetryAsync(() => _usersClient.CreateUserAsync(userCreateDto), logger: _logger);

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
            await RetryHelper.RetryAsync(() => _repository.AddUserCredentialsAsync(credentials));
            await RetryHelper.RetryAsync(() => _repository.SaveChangesAsync());
            _logger.LogInformation("User credentials for email {Email} saved successfully", credentials.Email);

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