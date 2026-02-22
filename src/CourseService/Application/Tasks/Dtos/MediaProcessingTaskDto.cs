using Courses.Domain.MediaProcessingTask.Primitives;

namespace Courses.Application.Tasks.Dtos;

public sealed record MediaProcessingTaskDto(
    Guid Id,
    Guid OriginalLessonId,
    Guid? AssignedTo,
    EditingJobStatus Status,
    string Message,
    IReadOnlyList<string> InputRawResourcePaths,
    IReadOnlyList<string> OutputResourcePaths,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? AssignedAtUtc,
    DateTimeOffset? CompletedAtUtc);
