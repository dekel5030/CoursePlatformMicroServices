using AuthService.Dtos;
using Common;

namespace AuthService.Services;

public interface IAuthService
{
    Result<CurrentUserReadDto> GetCurrentUser();
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto);

}