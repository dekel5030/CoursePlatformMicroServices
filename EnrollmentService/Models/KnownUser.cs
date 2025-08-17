namespace EnrollmentService.Models;

public class KnownUser
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }  
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
