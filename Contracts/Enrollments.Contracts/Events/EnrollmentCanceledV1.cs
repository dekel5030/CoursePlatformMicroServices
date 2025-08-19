namespace Enrollments.Contracts.Events;

public sealed record EnrollmentCancelledV1(
    int EnrollmentId,
    int UserId,
    int CourseId,
    string ReasonCode
)
{
    public const int Version = 1;
};