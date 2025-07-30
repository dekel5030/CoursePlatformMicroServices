using AuthService.Dtos;

namespace AuthService.Security;

public interface ITokenService
{
    string GenerateToken(TokenRequestDto request);
}