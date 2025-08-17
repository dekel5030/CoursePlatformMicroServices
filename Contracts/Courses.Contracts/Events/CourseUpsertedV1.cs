namespace Courses.Contracts.Events;

public sealed record CourseUpsertedV1(
    int CourseId,
    bool IsPublished
)
{
    public const string EventType = "CourseUpserted";
    public const int Version = 1;
};
