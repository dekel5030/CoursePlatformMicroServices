namespace Application.Abstractions.Security;

public interface ITokenService
{
    string GenerateToken(TokenRequestDto tokenRequest);
}

public class TokenRequestDto
{
    public required string Email { get; set; }
    public required IEnumerable<string> Roles { get; set; }
    public required IEnumerable<string> Permissions { get; set; }
}
