namespace Enrollments.Contracts.Events;

public sealed record EnrollmentCreatedV1(
    Guid MessageId,
    Guid CorrelationId,
    int EnrollmentId,
    int UserId,
    int CourseId,
    DateTime EnrolledAtUtc
);