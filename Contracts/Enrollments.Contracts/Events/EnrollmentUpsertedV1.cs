namespace Enrollments.Contracts.Events;

public sealed record EnrollmentUpsertedV1(
    int EnrollmentId,
    int UserId,
    int CourseId,
    bool IsActive
)
{
    public const int Version = 1;
};