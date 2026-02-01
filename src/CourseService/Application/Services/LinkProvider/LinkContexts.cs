using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Ratings.Primitives;

namespace Courses.Application.Services.LinkProvider;

public sealed record CourseContext(
    CourseId Id,
    UserId InstructorId,
    CourseStatus Status
) : ILinkEligibilityContext
{
    public Guid ResourceId => Id.Value;
    public Guid? OwnerId => InstructorId.Value;
    object? ILinkEligibilityContext.Status => Status;
}

internal sealed record ModuleContext(
    CourseContext Course,
    ModuleId Id) : ILinkEligibilityContext
{
    public Guid ResourceId => Id.Value;
    public Guid? OwnerId => Course.InstructorId.Value;
    object? ILinkEligibilityContext.Status => Course.Status;
}

internal sealed record LessonContext(
    CourseContext Course,
    LessonId Id,
    LessonAccess Access,
    bool HasEnrollment) : ILinkEligibilityContext
{
    public Guid ResourceId => Id.Value;
    public Guid? OwnerId => Course.InstructorId.Value;
    object? ILinkEligibilityContext.Status => Access;
}

internal sealed record CourseCollectionContext(
    PagedQueryDto Query,
    int TotalCount) : ILinkEligibilityContext
{
    public Guid ResourceId => Guid.Empty;
    public Guid? OwnerId => null;
    public object? Status => null;
}

internal sealed record CourseRatingEligibilityContext(
    CourseId CourseId,
    UserId? CurrentUserId,
    bool UserHasExistingRating) : ILinkEligibilityContext
{
    public Guid ResourceId => CourseId.Value;
    public Guid? OwnerId => null;
    public object? Status => UserHasExistingRating;
}

internal sealed record CourseRatingLinkContext(
    RatingId RatingId,
    UserId OwnerIdentifier,
    UserId? CurrentUserId) : ILinkEligibilityContext
{
    public Guid ResourceId => RatingId.Value;
    public Guid? OwnerId => OwnerIdentifier.Value;
    public object? Status => null;
}
