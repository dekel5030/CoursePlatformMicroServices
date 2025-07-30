namespace AuthService.Security;

public interface ITokenService
{
    string GenerateToken(int userId, string email);
}