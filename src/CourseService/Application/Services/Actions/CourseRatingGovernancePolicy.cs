using Courses.Application.Services.LinkProvider;

namespace Courses.Application.Services.Actions;

internal static class CourseRatingGovernancePolicy
{
#pragma warning disable S3400 
    public static bool CanReadRatings() => true;
#pragma warning restore S3400 

    public static bool CanCreateRating(CourseRatingCollectionContext context) =>
        context.CurrentUserId is not null && !context.UserHasExistingRating;

    public static bool CanUpdateRating(CourseRatingLinkContext context) =>
        context.CurrentUserId is not null &&
        context.OwnerIdentifier.Value == context.CurrentUserId.Value;

    public static bool CanDeleteRating(CourseRatingLinkContext context) =>
        context.CurrentUserId is not null &&
        context.OwnerIdentifier.Value == context.CurrentUserId.Value;
}
