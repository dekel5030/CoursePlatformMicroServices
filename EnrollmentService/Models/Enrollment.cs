using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Models;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }
    public int UserId { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    public DateTime EnrolledAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ExpiresAt { get; set; }
}