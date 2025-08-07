using Common.Auth;

namespace AuthService.Dtos;

public class TokenRequestDto
{
    public int UserId { get; init; }
    public required string Email { get; init; } 
    public string? FullName { get; init; }
    public required UserRole Role { get; init; }
}