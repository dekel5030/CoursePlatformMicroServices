namespace AuthService;

public class CurrentUserReadDto
{
    public int UserId { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
}