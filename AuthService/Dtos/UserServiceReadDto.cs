namespace AuthService.Dtos;

public class UserServiceReadDto
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
}