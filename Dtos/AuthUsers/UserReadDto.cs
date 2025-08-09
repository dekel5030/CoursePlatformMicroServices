using AuthService.Models;

namespace AuthService.Dtos.AuthUsers;

public class UserReadDto
{
    public int Id { get; set; }

    public required int UserId { get; set; }
    public string? Email { get; set; }

    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<UserPermission>? UserPermissions { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; } 
    public bool IsConfirmed { get; set; } 
    public int FailedLoginAttempts { get; set; } 

    public DateTime? LockedUntil { get; set; }
}