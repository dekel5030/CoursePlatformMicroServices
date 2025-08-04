using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class UserCredentials
{
    [Key]
    public int Id { get; private set; }

    public required int UserId { get; set; }
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    public DateTime UpdatedAt { get; internal set; }

    public bool IsActive { get; set; } = true;
    public bool IsConfirmed { get; set; } = false;
    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockedUntil { get; set; } = null;    
}

public enum UserRole
{
    User,
    Admin,
    SuperAdmin
}