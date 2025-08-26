using EnrollmentService.Models;

namespace EnrollmentService.Dtos;

public sealed record class EnrollmentReadDto
{
    public required int Id { get; init; }

    public required int CourseId { get; init; }
    public required int UserId { get; init; }

    public required EnrollmentStatus Status { get; init; }

    public required DateTime EnrolledAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}