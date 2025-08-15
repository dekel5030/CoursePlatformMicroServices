namespace EnrollmentService.Models;

public class KnownUser
{
    public int UserId { get; set; }
    public long Version { get; set; }
    public bool IsActive { get; set; }  
    public DateTime UpdatedAtUtc { get; set; }
}
