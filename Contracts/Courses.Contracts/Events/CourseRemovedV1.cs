namespace Courses.Contracts.Events;

public sealed record CourseRemovedV1(
    Guid MessageId,
    Guid CorrelationId,
    int CourseId,
    long Version,
    DateTime UpdatedAtUtc
);
