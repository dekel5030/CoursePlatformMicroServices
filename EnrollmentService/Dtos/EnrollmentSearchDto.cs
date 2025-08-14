using EnrollmentService.Models;

namespace EnrollmentService.Dtos;

public sealed record class EnrollmentSearchDto
{
    public int? Id { get; init; }

    public int? CourseId { get; init; }
    public int? UserId { get; init; }

    public EnrollmentStatus? Status { get; init; }

    public DateTime? EnrolledAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }

    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}