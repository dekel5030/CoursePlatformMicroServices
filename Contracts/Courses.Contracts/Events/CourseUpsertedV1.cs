namespace Courses.Contracts.Events;

public sealed record CourseUpsertedV1(
    Guid MessageId,
    Guid CorrelationId,
    int CourseId,
    long Version,
    DateTime UpdatedAtUtc
)
{
    public const string EventType = "CourseUpserted";
    public const int Version = 1;
};
