namespace Courses.Contracts.Events;

public sealed record CourseUpsertedV1(
    int CourseId,
    bool IsPublished
)
{
    public const int Version = 1;
};
