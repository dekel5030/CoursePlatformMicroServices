using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

internal sealed class CourseRatingCollectionLinkDefinitions : ILinkDefinitionRegistry
{
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public LinkResourceKey ResourceKey => LinkResourceKey.CourseRatingCollection;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<CourseRatingCollectionContext>(
                rel: LinkRels.CourseRating.Ratings,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourseRatings,
                policyCheck: _ => CourseRatingGovernancePolicy.CanReadRatings(),
                getRouteValues: ctx => new { courseId = ctx.CourseId.Value }),

            new LinkDefinition<CourseRatingCollectionContext>(
                rel: LinkRels.CourseRating.CreateRating,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.CreateCourseRating,
                policyCheck: ctx => CourseRatingGovernancePolicy.CanCreateRating(ctx),
                getRouteValues: ctx => new { courseId = ctx.CourseId.Value }),
        }.AsReadOnly();

        return _definitions;
    }
}
