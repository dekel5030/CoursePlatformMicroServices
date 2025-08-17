namespace CourseService.Models;

public class Enrollment
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Version { get; set; }
}
