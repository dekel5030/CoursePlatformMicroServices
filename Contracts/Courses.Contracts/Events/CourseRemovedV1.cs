namespace Courses.Contracts.Events;

public sealed record CourseRemovedV1(
    int CourseId
)
{
    public const string EventType = "CourseRemoved";
    public const int Version = 1;
};
