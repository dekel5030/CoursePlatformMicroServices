namespace Application.AuthUsers.Dtos;

public class AuthResponseDto
{
    public required Guid AuthUserId { get; set; }
    public required Guid UserId { get; set; } // Same as AuthUserId - unified ID
    public required string Email { get; set; }
    public required IEnumerable<string> Roles { get; set; }
    public required IEnumerable<string> Permissions { get; set; }
    public string? Message { get; set; }
}
