namespace Enrollments.Contracts.Events;

public sealed record EnrollmentUpserted(
    string EnrollmentId,
    string UserId,
    string CourseId,
    bool IsActive
);