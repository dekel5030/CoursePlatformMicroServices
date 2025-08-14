namespace AuthService.Models;

public class UserRole
{
    public int UserId { get; set; }
    public AuthUser User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}