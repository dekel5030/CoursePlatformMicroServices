using Courses.Domain.Abstractions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.MediaProcessingTask.Errors;
using Courses.Domain.MediaProcessingTask.Events;
using Courses.Domain.MediaProcessingTask.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.MediaProcessingTask;

public sealed class MediaProcessingTask : Entity<TaskId>, IAuditable
{
    public override TaskId Id { get; protected set; } = TaskId.NewId();
    public LessonId OriginalLessonId { get; private set; }
    public UserId? AssignedTo { get; private set; }

    private readonly List<Url> _inputRawResources = [];
    public IReadOnlyList<Url> InputRawResources => _inputRawResources.AsReadOnly();

    private readonly List<Url> _outputResources = [];
    public IReadOnlyList<Url> OutputResources => _outputResources.AsReadOnly();

    public EditingJobStatus Status { get; private set; } = EditingJobStatus.Pending;
    public string Message { get; private set; }

    public DateTimeOffset? AssignedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }

    private MediaProcessingTask(LessonId originalLessonId, string message, IEnumerable<Url> resources)
    {
        OriginalLessonId = originalLessonId;
        Message = message;
        _inputRawResources.AddRange(resources);
    }

    public static MediaProcessingTask Create(LessonId originalLessonId, string message, IEnumerable<Url> resources)
    {
        return new MediaProcessingTask(originalLessonId, message, resources);
    }

    public Result Assign(UserId userId, ITimeProvider timeProvider)
    {
        if (Status != EditingJobStatus.Pending)
        {
            return Result.Failure(EditingJobErrors.JobAlreadyAssigned);
        }

        AssignedTo = userId;
        Status = EditingJobStatus.InProgress;
        AssignedAtUtc = timeProvider.UtcNow;

        return Result.Success();
    }

    public Result AddOutputResource(Url outputResource)
    {
        if (Status != EditingJobStatus.InProgress)
        {
            return Result.Failure(EditingJobErrors.JobNotInProgress);
        }

        if (_outputResources.Contains(outputResource))
        {
            return Result.Success();
        }

        _outputResources.Add(outputResource);
        return Result.Success();
    }

    public Result Complete(string message, ITimeProvider timeProvider)
    {
        if (Status != EditingJobStatus.InProgress)
        {
            return Result.Failure(EditingJobErrors.JobNotInProgress);
        }

        Status = EditingJobStatus.Completed;
        CompletedAtUtc = timeProvider.UtcNow;
        Message = message;

        Raise(new MediaProcesingTaskCompletedDomainEvent(
            Id,
            OriginalLessonId,
            message,
            InputRawResources,
            OutputResources));

        return Result.Success();
    }
}
