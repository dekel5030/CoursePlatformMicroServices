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
using UserRoleType = Common.Auth.UserRole;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRollbackManager _rollbackManager;
    private readonly ILogger<AuthService> _logger;
    private readonly UnitOfWork _unitOfWork;
    private readonly IGrpcUserServiceDataClient _usersClient;

    public AuthService( IMapper mapper,
                        IPasswordHasher passwordHasher,
                        ITokenService tokenService,
                        IRollbackManager rollbackManager,
                        ILogger<AuthService> logger,
                        IGrpcUserServiceDataClient usersClient,
                        UnitOfWork unitOfWork)
    {
        _usersClient = usersClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _rollbackManager = rollbackManager;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        if (await _unitOfWork.AuthUserRepository.GetUserByEmailAsync(registerRequestDto.Email) != null)
        {
            _logger.LogInformation("Registration attempt with existing email: {Email}", registerRequestDto.Email);
            return Result<AuthResponseDto>.Failure(Error.DuplicateEmail);
        }

        var userCreateDto = _mapper.Map<UserCreateDto>(registerRequestDto);
        userCreateDto.PasswordHash = _passwordHasher.Hash(registerRequestDto.Password);

        var userServiceResult = await CreateUserInUserServiceAsync(userCreateDto);
        if (!userServiceResult.IsSuccess)
        {
            return Result<AuthResponseDto>.Failure(userServiceResult.Error!);
        }

        _rollbackManager.Add(() => RetryHelper.RetryAsync(() => _usersClient.DeleteUserAsync(userServiceResult.Value!.Id)));
        var userReadDto = userServiceResult.Value;
        var authUser = await CreateAuthUser(userReadDto!, userCreateDto.PasswordHash);

        return await TrySaveAuthUserAndRespondAsync(authUser, registerRequestDto.Email);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var authUser = await _unitOfWork.AuthUserRepository.GetUserWithAccessDataByEmailAsync(loginRequestDto.Email);

        if (authUser == null)
        {
            _logger.LogInformation("Login attempt with non-existing email: {Email}", loginRequestDto.Email);
            return Result<AuthResponseDto>.Failure(Error.InvalidCredentials);
        }

        if (!_passwordHasher.VerifyPassword(loginRequestDto.Password, authUser.PasswordHash))
        {
            _logger.LogInformation("Login attempt with invalid password for email: {Email}", loginRequestDto.Email);
            return Result<AuthResponseDto>.Failure(Error.InvalidCredentials);
        }

        return Result<AuthResponseDto>.Success(CreateAuthResponseDto(authUser));
    }

    private async Task<AuthUser> CreateAuthUser(UserServiceReadDto userReadDto, string hashedPassword)
    {
        var role = await _unitOfWork.RoleRepository.GetRoleByNameAsync(UserRoleType.User.ToString());

        if (role is null)
        {
            _logger.LogInformation("Role not found: {Role}", UserRoleType.User);
            throw new InvalidOperationException($"Default role '{UserRoleType.User}' was not found in the database.");
        }

        var userRole = new UserRole { RoleId = role!.Id };
        var authUser = _mapper.Map<AuthUser>(userReadDto);
        authUser.PasswordHash = hashedPassword;
        authUser.UserRoles = new List<UserRole>() { userRole };

        return authUser;
    }

    private AuthResponseDto CreateAuthResponseDto(AuthUser authUser)
    {
        var authResponseDto = _mapper.Map<AuthResponseDto>(authUser);
        var tokenRequestDto = CreateTokenRequestDto(authUser);

        authResponseDto.Token = _tokenService.GenerateToken(tokenRequestDto);

        return authResponseDto;
    }

    private TokenRequestDto CreateTokenRequestDto(AuthUser authUser)
    {
        var tokenRequestDto = _mapper.Map<TokenRequestDto>(authUser);

        var allPermissions = new HashSet<Permission>();
        
        foreach (var userRole in authUser.UserRoles)
        {
            var role = userRole.Role;

            foreach (var rolePermission in role.RolePermissions)
            {
                allPermissions.Add(rolePermission.Permission);
            }
        }

        foreach (var userPermission in authUser.UserPermissions)
        {
            allPermissions.Add(userPermission.Permission);
        }

        tokenRequestDto.Permissions = allPermissions;
        return tokenRequestDto;
    }

    private async Task<Result<UserServiceReadDto>> CreateUserInUserServiceAsync(UserCreateDto userCreateDto)
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

    private async Task<Result<AuthResponseDto>> TrySaveAuthUserAndRespondAsync(AuthUser authUser, string email)
    {
        try
        {   
            await RetryHelper.RetryAsync(() => _unitOfWork.AuthUserRepository.AddUserAsync(authUser));
            await RetryHelper.RetryAsync(() => _unitOfWork.SaveChangesAsync());
            _logger.LogInformation("User authUser for email {Email} saved successfully", authUser.Email);

            var fullAuthUser = await RetryHelper
            .RetryAsync(() => _unitOfWork.AuthUserRepository.GetUserWithAccessDataByEmailAsync(authUser.Email));
            return Result<AuthResponseDto>.Success(CreateAuthResponseDto(fullAuthUser!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving user authUser for email {Email}", email);
            await _rollbackManager.ExecuteAllAsync();

            return Result<AuthResponseDto>.Failure(Error.DatabaseError);
        }
    }
}