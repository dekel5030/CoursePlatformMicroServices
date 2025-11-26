namespace Application.Enrollments.Commands.CreateEnrollment;

public record CreateEnrollmentDto(
    string UserId,
    string CourseId,
    DateTime ExpiresAt);