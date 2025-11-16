namespace Application.AuthUsers.Dtos;

public class AuthResponseDto
{
    public required Guid AuthUserId { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required IEnumerable<string> Roles { get; set; }
    public required IEnumerable<string> Permissions { get; set; }
}
