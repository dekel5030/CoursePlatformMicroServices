namespace Courses.Application.Enrollments.Dtos;

public sealed record EnrollmentDto(
    Guid Id,
    Guid CourseId,
    Guid StudentId,
    DateTimeOffset EnrolledAt,
    DateTimeOffset ExpiresAt,
    string Status,
    DateTimeOffset? CompletedAt);
