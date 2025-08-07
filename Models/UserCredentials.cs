using System.ComponentModel.DataAnnotations;
using Common.Auth;

namespace AuthService.Models;

public class UserCredentials
{
    [Key]
    public int Id { get; private set; }

    public required int UserId { get; set; }
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }
    //public UserRole Role { get; set; } = UserRole.User;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public DateTime UpdatedAt { get; internal set; }

    public bool IsActive { get; set; } = true;
    public bool IsConfirmed { get; set; } = false;
    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockedUntil { get; set; } = null;    
}