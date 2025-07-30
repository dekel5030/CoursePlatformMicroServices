using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class UserCredentials
{
    [Key]
    public int Id { get; private set; }

    public required int UserId { get; set; }
    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string PasswordSalt { get; set; }

    public DateTime CreatedAt { get; private set; } 

    public DateTime UpdatedAt { get; private set; }

    public bool IsActive { get; set; } = true;

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockedUntil { get; set; } = null;
}