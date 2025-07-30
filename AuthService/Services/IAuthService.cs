using AuthService.Dtos;
using Common;

namespace AuthService.Services;

public interface IAuthServie
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto);
}