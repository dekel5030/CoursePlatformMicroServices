namespace Application.AuthUsers.Commands.RegisterUser;

public class RegisterRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string UserName { get; set; }
    public string? Fullname { get; set; }
    public string? PhoneNumber { get; set; }
}
