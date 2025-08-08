namespace AuthService.Models;

public class UserPermission
{
    public int UserId { get; set; }
    public AuthUser User { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}