namespace CoursePlatform.Contracts.CourseEvents;

public sealed record CourseUpserted(
    string CourseId,
    string Title,
    bool IsPublished
);