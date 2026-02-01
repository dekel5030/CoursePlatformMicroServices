using Courses.Application.Services.LinkProvider;

namespace Courses.Application.Services.Actions;

/// <summary>
/// Policy that decides when course rating actions (create, read, update, delete) are allowed.
/// Used by link definitions to build links based on context.
/// </summary>
internal static class CourseRatingGovernancePolicy
{
    /// <summary>
    /// Consolidated entry point for link eligibility checks.
    /// Handles both CourseRatingEligibilityContext (GetCourseById) and CourseRatingLinkContext (GetCourseRatings).
    /// </summary>
    public static bool Can(CourseRatingAction action, object context)
    {
        if (context is CourseRatingEligibilityContext eligibilityContext)
        {
            return action switch
            {
                CourseRatingAction.ReadRatings => true,
                CourseRatingAction.Create => eligibilityContext.CurrentUserId is not null && !eligibilityContext.UserHasExistingRating,
                _ => false
            };
        }

        if (context is CourseRatingLinkContext linkContext)
        {
            return action switch
            {
                CourseRatingAction.Update => linkContext.CurrentUserId is not null &&
                    linkContext.OwnerIdentifier.Value == linkContext.CurrentUserId.Value,
                CourseRatingAction.Delete => linkContext.CurrentUserId is not null &&
                    linkContext.OwnerIdentifier.Value == linkContext.CurrentUserId.Value,
                _ => false
            };
        }

        return false;
    }
}
