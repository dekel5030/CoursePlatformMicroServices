using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider;

internal sealed class CourseCollectionLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public CourseCollectionLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public string ResourceKey => LinkResourceKeys.CourseCollection;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<CourseCollectionContext>(
                rel: "self",
                method: "GET",
                endpointName: "GetCourses",
                policyCheck: _ => true,
                getRouteValues: ctx => ctx.Query),

            new LinkDefinition<CourseCollectionContext>(
                rel: "next-page",
                method: "GET",
                endpointName: "GetCourses",
                policyCheck: ctx => (ctx.Query.Page ?? 1) * (ctx.Query.PageSize ?? 10) < ctx.TotalCount,
                getRouteValues: ctx => ctx.Query with { Page = (ctx.Query.Page ?? 1) + 1 }),

            new LinkDefinition<CourseCollectionContext>(
                rel: "previous-page",
                method: "GET",
                endpointName: "GetCourses",
                policyCheck: ctx => (ctx.Query.Page ?? 1) > 1,
                getRouteValues: ctx => ctx.Query with { Page = (ctx.Query.Page ?? 1) - 1 }),

            new LinkDefinition<CourseCollectionContext>(
                rel: "create",
                method: "POST",
                endpointName: "CreateCourse",
                policyCheck: _ => _policy.CanCreateCourse(),
                getRouteValues: _ => (object?)null)
        }.AsReadOnly();

        return _definitions;
    }
}
