using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Security;
using AuthService.SyncDataServices.Http;
using AutoMapper;
using Common;
using Common.Errors;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IUserServiceDataClient _usersClient;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IAuthRepository repository,
                        IUserServiceDataClient usersClient,
                        IMapper mapper,
                        IPasswordHasher passwordHasher,
                        ITokenService tokenService)
    {
        _repository = repository;
        _usersClient = usersClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var existingCredentials = await _repository.GetUserCredentialsByEmailAsync(registerRequestDto.Email);

        if (existingCredentials != null)
        {
            return Result<AuthResponseDto>.Failure(Error.DuplicateEmail);
        }

        var userCreateDto = _mapper.Map<UserCreateDto>(registerRequestDto);
        var result = await _usersClient.CreateUserAsync(userCreateDto);

        if (!result.IsSuccess)
        {
            return Result<AuthResponseDto>.Failure(result.Error!);
        }

        var userReadDto = result.Value;
        var userCredentials = CreateUserCredentials(userReadDto!, registerRequestDto.Password);

        await _repository.AddUserCredentialsAsync(userCredentials);
        await _repository.SaveChangesAsync();

        var authResponseDto = GenerateAuthResponseDto(userCredentials);

        return Result<AuthResponseDto>.Success(authResponseDto);
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
}