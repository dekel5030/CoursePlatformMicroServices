using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider.Contracts;

/// <summary>
/// Link definitions for CourseRating context.
/// Used in GetCourseRatings to add update/delete links per rating according to policy.
/// </summary>
internal sealed class CourseRatingLinkDefinitions : ILinkDefinitionRegistry
{
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public LinkResourceKey ResourceKey => LinkResourceKey.CourseRating;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<CourseRatingLinkContext>(
                rel: LinkRels.PartialUpdate,
                method: LinkHttpMethod.Patch,
                endpointName: EndpointNames.UpdateCourseRating,
                policyCheck: ctx => CourseRatingGovernancePolicy.Can(CourseRatingAction.Update, ctx),
                getRouteValues: ctx => new { ratingId = ctx.RatingId.Value }),

            new LinkDefinition<CourseRatingLinkContext>(
                rel: LinkRels.Delete,
                method: LinkHttpMethod.Delete,
                endpointName: EndpointNames.DeleteCourseRating,
                policyCheck: ctx => CourseRatingGovernancePolicy.Can(CourseRatingAction.Delete, ctx),
                getRouteValues: ctx => new { ratingId = ctx.RatingId.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
