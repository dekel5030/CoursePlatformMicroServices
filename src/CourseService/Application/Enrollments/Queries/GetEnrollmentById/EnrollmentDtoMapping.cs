using Courses.Application.Enrollments.Dtos;
using Courses.Domain.Enrollments;

namespace Courses.Application.Enrollments.Queries.GetEnrollmentById;

internal static class EnrollmentDtoMapping
{
    internal static EnrollmentDto Map(Enrollment enrollment)
    {
        return new EnrollmentDto(
            enrollment.Id.Value,
            enrollment.CourseId.Value,
            enrollment.StudentId.Value,
            enrollment.EnrolledAt,
            enrollment.ExpiresAt,
            enrollment.Status.ToString(),
            enrollment.CompletedAt,
            enrollment.LastAccessedLessonId?.Value,
            enrollment.LastAccessedAt,
            enrollment.LastWatchedSecond);
    }
}
