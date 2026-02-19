using Courses.Domain.Abstractions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.MediaPackages.Primitives;
using Courses.Domain.Shared;
using Kernel;

namespace Courses.Domain.EditingJob;

public sealed class EditingJob : Entity<EditingJobId>, IAuditable
{
    public override EditingJobId Id { get; protected set; } = EditingJobId.NewId();
    public MediaPackageId MediaPackageId { get; private set; }
    public CourseId CourseId { get; private set; }
    public UserId? AssignedTo { get; private set; }

    public EditingJobStatus Status { get; private set; } = EditingJobStatus.Pending;
    public string Message { get; private set; } = string.Empty;

    public DateTimeOffset? AssignedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }

    private EditingJob(MediaPackageId mediaPackageId, CourseId courseId)
    {
        MediaPackageId = mediaPackageId;
        CourseId = courseId;
    }

    public static EditingJob Create(MediaPackageId mediaPackageId, CourseId courseId)
    {
        return new EditingJob(mediaPackageId, courseId);
    }

    public Result Assign(UserId adminId, ITimeProvider timeProvider)
    {
        if (Status != EditingJobStatus.Pending)
        {
            return Result.Failure(EditingJobErrors.JobAlreadyAssigned);
        }

        AssignedTo = adminId;
        Status = EditingJobStatus.InProgress;
        AssignedAtUtc = timeProvider.UtcNow;

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

        Raise(new EditingJobCompletedDomainEvent(Id, MediaPackageId, CourseId, Message));

        return Result.Success();
    }
}
