namespace Application.Abstractions.Security;

public interface ITokenService
{
    string GenerateToken(TokenRequestDto tokenRequest);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string refreshToken);
    string HashRefreshToken(string refreshToken);
}

public class TokenRequestDto
{
    public required string Email { get; set; }
    public required IEnumerable<string> Roles { get; set; }
    public required IEnumerable<string> Permissions { get; set; }
}

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshTokenExpiresAt { get; set; }
}
