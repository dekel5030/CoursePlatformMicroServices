namespace Courses.Domain.MediaProcessingTask.Primitives;

public sealed record TaskId(Guid Value)
{
    public static TaskId NewId() => new(Guid.CreateVersion7());
}
