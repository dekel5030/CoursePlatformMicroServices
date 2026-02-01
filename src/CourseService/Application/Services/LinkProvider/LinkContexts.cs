using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings.Primitives;

namespace Courses.Application.Services.LinkProvider;

internal sealed record ModuleLinkContext(
    CourseState CourseState,
    ModuleState ModuleState) : ILinkEligibilityContext
{
    public Guid ResourceId => ModuleState.Id.Value;
    public Guid? OwnerId => CourseState.InstructorId.Value;
    object? ILinkEligibilityContext.Status => CourseState.Status;
}

internal sealed record LessonLinkContext(
    CourseState CourseState,
    ModuleState ModuleState,
    LessonState LessonState,
    EnrollmentState? EnrollmentState) : ILinkEligibilityContext
{
    public Guid ResourceId => LessonState.Id.Value;
    public Guid? OwnerId => CourseState.InstructorId.Value;
    object? ILinkEligibilityContext.Status => LessonState.LessonAccess;
}

internal sealed record CourseCollectionContext(
    PagedQueryDto Query,
    int TotalCount) : ILinkEligibilityContext
{
    public Guid ResourceId => Guid.Empty;
    public Guid? OwnerId => null;
    public object? Status => null;
}

/// <summary>
/// Context for rating eligibility links in GetCourseById response.
/// Used to build "ratings" (GET) and "create-rating" (POST) links.
/// </summary>
internal sealed record CourseRatingEligibilityContext(
    CourseId CourseId,
    UserId? CurrentUserId,
    bool UserHasExistingRating);

/// <summary>
/// Context for per-rating links in GetCourseRatings response.
/// Used to build update/delete links for each rating according to policy.
/// </summary>
internal sealed record CourseRatingLinkContext(
    RatingId RatingId,
    UserId OwnerIdentifier,
    UserId? CurrentUserId) : ILinkEligibilityContext
{
    public Guid ResourceId => RatingId.Value;
    public Guid? OwnerId => OwnerIdentifier.Value;
    public object? Status => null;
}
