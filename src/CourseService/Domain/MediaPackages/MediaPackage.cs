using Courses.Domain.Courses.Primitives;
using Courses.Domain.MediaPackages.Errors;
using Courses.Domain.MediaPackages.Events;
using Courses.Domain.MediaPackages.Primitives;
using Courses.Domain.Shared;
using Kernel;

namespace Courses.Domain.MediaPackages;

public sealed class MediaPackage : Entity<MediaPackageId>, IAuditable
{
    public override MediaPackageId Id { get; protected set; } = MediaPackageId.NewId();
    public CourseId CourseId { get; private set; }
    public UserId InstructorId { get; private set; }

    public IReadOnlyList<RawAsset> Assets => _assets.AsReadOnly();
    private readonly List<RawAsset> _assets = [];

    public string? Message { get; private set; }
    public MediaPackageStatus Status { get; private set; } = MediaPackageStatus.Draft;

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }


    private MediaPackage(CourseId courseId, UserId instructorId, string? message)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        Message = message;
    }

    public static Result<MediaPackage> Create(CourseId courseId, UserId instructorId, string? message)
    {
        var mediaPackage = new MediaPackage(courseId, instructorId, message);

        return Result.Success(mediaPackage);
    }

    public Result AddAssets(IEnumerable<RawAsset> assets)
    {
        if (Status == MediaPackageStatus.Published)
        {
            return Result.Failure(MediaPackageErrors.CannotChangePublishedPackage);
        }

        _assets.AddRange(assets);

        return Result.Success();
    }

    public Result Publish()
    {
        if (Status == MediaPackageStatus.Published)
        {
            return Result.Failure(MediaPackageErrors.CannotChangePublishedPackage);
        }

        if (_assets.Count == 0)
        {
            return Result.Failure(MediaPackageErrors.NoAssets);
        }

        Status = MediaPackageStatus.Published;

        Raise(new MediaPackagePublishedDomainEvent(Id, CourseId, InstructorId, _assets, Message));

        return Result.Success();
    }

    public void UpdateMessage(string? message)
    {
        if (Status == MediaPackageStatus.Published)
        {
            return;
        }

        Message = message;
    }
}
