namespace AuthService.Dtos.AuthUsers;

public class UserSearch
{
    public int? Id { get; set; }

    public int? UserId { get; set; }
    public string? Email { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsConfirmed { get; set; }
    public int? FailedLoginAttempts { get; set; }

    public DateTime? LockedUntil { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}