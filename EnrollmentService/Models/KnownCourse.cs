namespace EnrollmentService.Models;

public class KnownCourse
{
    public int CourseId { get; set; }
    public long Version { get; set; }
    public bool IsAvailable { get; set; }  
    public DateTime UpdatedAtUtc { get; set; }
}
