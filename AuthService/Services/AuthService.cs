using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Security;
using AuthService.SyncDataServices.Http;
using AutoMapper;
using Common;

namespace AuthService.Services;

public class AuthService : IAuthServie
{
    private readonly IAuthRepository _repository;
    private readonly IUserServiceDataClient _usersClient;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IAuthRepository repository,
                        IUserServiceDataClient usersClient,
                        IMapper mapper,
                        IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _usersClient = usersClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
    {
        var userCreateDto = _mapper.Map<UserCreateDto>(registerRequestDto);
        userCreateDto.PasswordHash = _passwordHasher.Hash(registerRequestDto.Password);

        var result = await _usersClient.CreateUserAsync(userCreateDto);

        if (!result.IsSuccess)
        {
            return Result<AuthResponseDto>.Failure(result.Error!);
        }

        var userReadDto = result.Value;
        var userCredentials = _mapper.Map<UserCredentials>(userReadDto);
        userCredentials.PasswordHash = userCreateDto.PasswordHash;

        var authResponseDto = _mapper.Map<AuthResponseDto>(userCredentials);

        await _repository.AddUserCredentialsAsync(userCredentials);
        await _repository.SaveChangesAsync();

        return Result<AuthResponseDto>.Success(authResponseDto);

    }
}