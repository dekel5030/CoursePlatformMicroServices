using AuthService.Dtos;
using Common;

namespace AuthService.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto);

}