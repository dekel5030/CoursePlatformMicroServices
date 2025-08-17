namespace Courses.Contracts.Events;

public sealed record CourseRemovedV1(
    Guid MessageId,
    Guid CorrelationId,
    int CourseId,
    long Version,
    DateTime UpdatedAtUtc
)
{
    public const string EventType = "CourseRemoved";
    public const int Version = 1;
};
