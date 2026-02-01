using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

internal sealed class CourseCollectionLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public CourseCollectionLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public LinkResourceKey ResourceKey => LinkResourceKey.CourseCollection;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<CourseCollectionContext>(
                rel: LinkRels.Self,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourses,
                policyCheck: _ => true,
                getRouteValues: ctx => ctx.Query),

            new LinkDefinition<CourseCollectionContext>(
                rel: LinkRels.Pagination.NextPage,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourses,
                policyCheck: ctx => CourseGovernancePolicy.CanShowNextPage(ctx),
                getRouteValues: ctx => ctx.Query with { Page = (ctx.Query.Page ?? 1) + 1 }),

            new LinkDefinition<CourseCollectionContext>(
                rel: LinkRels.Pagination.PreviousPage,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourses,
                policyCheck: ctx => CourseGovernancePolicy.CanShowPreviousPage(ctx),
                getRouteValues: ctx => ctx.Query with { Page = (ctx.Query.Page ?? 1) - 1 }),

            new LinkDefinition<CourseCollectionContext>(
                rel: LinkRels.Create,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.CreateCourse,
                policyCheck: _ => _policy.CanCreateCourse(),
                getRouteValues: _ => (object?)null)
        }.AsReadOnly();

        return _definitions;
    }
}
