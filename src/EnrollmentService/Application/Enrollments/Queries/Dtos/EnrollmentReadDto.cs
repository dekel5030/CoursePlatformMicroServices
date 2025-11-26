using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Queries.Dtos;

public record EnrollmentReadDto
{
    public string Id { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public string CourseId { get; init; } = null!;
    public EnrollmentStatus Status { get; init; }
    public DateTime EnrolledAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
