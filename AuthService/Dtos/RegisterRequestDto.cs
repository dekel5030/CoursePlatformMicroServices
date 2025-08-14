namespace AuthService.Dtos;

public class RegisterRequestDto
{
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}