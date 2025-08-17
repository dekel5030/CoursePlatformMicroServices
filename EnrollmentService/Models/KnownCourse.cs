namespace EnrollmentService.Models;

public class KnownCourse
{
    public int CourseId { get; set; }
    public bool IsAvailable { get; set; }  
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
