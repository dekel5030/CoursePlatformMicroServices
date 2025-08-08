using Common.Auth;

namespace AuthService.Dtos;

public class TokenRequestDto
{
    public int UserId { get; set; }
    public required string Email { get; set; } 
    public string? FullName { get; set; }
    public required ICollection<string> Permissions { get; set; }
}