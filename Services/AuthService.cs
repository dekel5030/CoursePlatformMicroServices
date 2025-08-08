using System.Security.Claims;
using System.Threading.Tasks;
using AuthService.Data.Repositories.Interfaces;
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
    private readonly IUserCredentialsRepository _credentialsRepo;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRollbackManager _rollbackManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserPermissionRepository _userPermissionRepo;
    private readonly IRolePermissionRepository _rolePermissionRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly IRoleRepository _roleRepository;
    private readonly IGrpcUserServiceDataClient _usersClient;

    public AuthService(IUserCredentialsRepository credentialsRepo,
                        IUserPermissionRepository userPermissionRepo,
                        IUserRoleRepository userRoleRepo,
                        IRolePermissionRepository rolePermissionRepo,
                        IRoleRepository roleRepository,
                        IMapper mapper,
                        IPasswordHasher passwordHasher,
                        ITokenService tokenService,
                        IRollbackManager rollbackManager,
                        ILogger<AuthService> logger,
                        IGrpcUserServiceDataClient usersClient,
                        IHttpContextAccessor httpContextAccessor)
    {
        _credentialsRepo = credentialsRepo;
        _usersClient = usersClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _rollbackManager = rollbackManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _userPermissionRepo = userPermissionRepo;
        _rolePermissionRepo = rolePermissionRepo;
        _userRoleRepo = userRoleRepo;
        _roleRepository = roleRepository;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        if (await _credentialsRepo.GetUserCredentialsByEmailAsync(registerRequestDto.Email) != null)
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
        var userCredentials = await CreateUserCredentials(userReadDto!, userCreateDto.PasswordHash);

        return await TrySaveCredentialsAndRespondAsync(userCredentials, registerRequestDto.Email);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var userCredentials = await _credentialsRepo.GetUserCredentialsByEmailAsync(loginRequestDto.Email, includeAccessData: true);

        if (userCredentials == null)
        {
            _logger.LogInformation("Login attempt with non-existing email: {Email}", loginRequestDto.Email);
            return Result<AuthResponseDto>.Failure(Error.InvalidCredentials);
        }

        if (!_passwordHasher.VerifyPassword(loginRequestDto.Password, userCredentials.PasswordHash))
        {
            _logger.LogInformation("Login attempt with invalid password for email: {Email}", loginRequestDto.Email);
            return Result<AuthResponseDto>.Failure(Error.InvalidCredentials);
        }

        return Result<AuthResponseDto>.Success(CreateAuthResponseDto(userCredentials));
    }

    public Result<CurrentUserReadDto> GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = user?.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = user?.FindFirst(ClaimTypes.Name)?.Value;
        var role = user?.FindFirst(ClaimTypes.Role)?.Value;

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            _logger.LogInformation("GetCurrentUser called but no valid userId found in token.");
            return Result<CurrentUserReadDto>.Failure(Error.UnAuthenticated);
        }

        var dto = new CurrentUserReadDto
        {
            UserId = userId,
            Email = email!,
            FullName = fullName,
            Role = role
        };

        return Result<CurrentUserReadDto>.Success(dto);
    }

    private async Task<UserCredentials> CreateUserCredentials(UserReadDto userReadDto, string hashedPassword)
    {
        var role = await _roleRepository.GetRoleByNameAsync(RoleType.User.ToString());

        if (role is null)
        {
            _logger.LogInformation("Role not found: {Role}", RoleType.User);
            throw new InvalidOperationException($"Default role '{RoleType.User}' was not found in the database.");
        }

        var userRole = new UserRole { RoleId = role!.Id };
        var credentials = _mapper.Map<UserCredentials>(userReadDto);
        credentials.PasswordHash = hashedPassword;
        credentials.UserRoles = new List<UserRole>() { userRole };

        return credentials;
    }

    private AuthResponseDto CreateAuthResponseDto(UserCredentials userCredentials)
    {
        var authResponseDto = _mapper.Map<AuthResponseDto>(userCredentials);
        var tokenRequestDto = CreateTokenRequestDto(userCredentials);

        authResponseDto.Token = _tokenService.GenerateToken(tokenRequestDto);

        return authResponseDto;
    }

    private TokenRequestDto CreateTokenRequestDto(UserCredentials userCredentials)
    {
        var tokenRequestDto = _mapper.Map<TokenRequestDto>(userCredentials);
        tokenRequestDto.Permissions = GetAllPermissions(userCredentials);

        return tokenRequestDto;
    }

    private static ICollection<string> GetAllPermissions(UserCredentials user)
    {
        var fromRoles = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name);

        var userSpecific = user.UserPermissions
            .Select(up => up.Permission.Name);

        return fromRoles
            .Union(userSpecific)
            .Distinct()
            .ToList();
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
            await RetryHelper.RetryAsync(() => _credentialsRepo.AddUserCredentialsAsync(credentials));
            await RetryHelper.RetryAsync(() => _credentialsRepo.SaveChangesAsync());
            _logger.LogInformation("User credentials for email {Email} saved successfully", credentials.Email);

            var fullCredentials = await RetryHelper
            .RetryAsync(() => _credentialsRepo.GetUserCredentialsByEmailAsync(credentials.Email, includeAccessData: true));
            return Result<AuthResponseDto>.Success(CreateAuthResponseDto(fullCredentials!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving user credentials for email {Email}", email);
            await _rollbackManager.ExecuteAllAsync();

            return Result<AuthResponseDto>.Failure(Error.DatabaseError);
        }
    }

    private enum RoleType
    {
        Admin,
        Instructor,
        User,
        Guest
    }
}

