namespace Enrollments.Contracts.Events;

public sealed record EnrollmentCreatedV1(
    int EnrollmentId,
    int UserId,
    int CourseId
)
{
    public const string EventType = "EnrollmentCreated";
    public const int Version = 1;
};