using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Queries.Dtos;

public record EnrollmentReadDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int CourseId { get; init; }
    public EnrollmentStatus Status { get; init; }
    public DateTime EnrolledAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
