using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

internal sealed class EnrolledCourseCollectionLinkDefinitions : ILinkDefinitionRegistry
{
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public LinkResourceKey ResourceKey => LinkResourceKey.EnrolledCourseCollection;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<EnrolledCourseCollectionContext>(
                rel: LinkRels.Self,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetEnrolledCourses,
                policyCheck: _ => true,
                getRouteValues: ctx => new { pageNumber = ctx.PageNumber, pageSize = ctx.PageSize }),

            new LinkDefinition<EnrolledCourseCollectionContext>(
                rel: LinkRels.Pagination.NextPage,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetEnrolledCourses,
                policyCheck: ctx => ctx.PageNumber * ctx.PageSize < ctx.TotalCount,
                getRouteValues: ctx => new { pageNumber = ctx.PageNumber + 1, pageSize = ctx.PageSize }),

            new LinkDefinition<EnrolledCourseCollectionContext>(
                rel: LinkRels.Pagination.PreviousPage,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetEnrolledCourses,
                policyCheck: ctx => ctx.PageNumber > 1,
                getRouteValues: ctx => new { pageNumber = ctx.PageNumber - 1, pageSize = ctx.PageSize })
        }.AsReadOnly();

        return _definitions;
    }
}
