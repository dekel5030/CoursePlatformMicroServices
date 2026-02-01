using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

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
                policyCheck: ctx => CourseRatingGovernancePolicy.CanUpdateRating(ctx),
                getRouteValues: ctx => new { ratingId = ctx.RatingId.Value }),

            new LinkDefinition<CourseRatingLinkContext>(
                rel: LinkRels.Delete,
                method: LinkHttpMethod.Delete,
                endpointName: EndpointNames.DeleteCourseRating,
                policyCheck: ctx => CourseRatingGovernancePolicy.CanDeleteRating(ctx),
                getRouteValues: ctx => new { ratingId = ctx.RatingId.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
