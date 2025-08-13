using EnrollmentService.Models;

namespace EnrollmentService.Dtos;

public class EnrollmentCreateDto
{
    public required int CourseId { get; set; }
    public required int UserId { get; set; }
    public required EnrollmentStatus Status { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
