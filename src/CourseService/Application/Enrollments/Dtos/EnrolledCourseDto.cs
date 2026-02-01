using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Enrollments.Dtos;

public sealed record EnrolledCourseDto
{
    public required Guid EnrollmentId { get; init; }
    public required Guid CourseId { get; init; }
    public required string CourseTitle { get; init; }
    public required string CourseSlug { get; init; }
    public required double ProgressPercentage { get; init; }
    public required DateTimeOffset? LastAccessedAt { get; init; }
    public required DateTimeOffset EnrolledAt { get; init; }
    public required string Status { get; init; }
    public required IReadOnlyList<LinkDto> Links { get; init; }
}
