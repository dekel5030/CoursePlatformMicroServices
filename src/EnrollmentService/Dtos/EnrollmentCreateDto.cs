using EnrollmentService.Models;

namespace EnrollmentService.Dtos;

public sealed record class EnrollmentCreateDto
{
    public required int CourseId { get; init; }
    public required int UserId { get; init; }
    public required EnrollmentStatus Status { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
