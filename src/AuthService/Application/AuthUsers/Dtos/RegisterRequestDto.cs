namespace Application.AuthUsers.Dtos;

public class RegisterRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Fullname { get; set; }
}
