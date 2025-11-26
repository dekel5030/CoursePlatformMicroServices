namespace Courses.Contracts.Events;

public sealed record CourseUpserted(
    string CourseId,
    string Title,
    bool IsPublished
);