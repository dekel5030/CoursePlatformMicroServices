using CoursePlatform.ServiceDefaults.Dtos;
using Courses.Api.Endpoints.Courses;
using Courses.Api.Infrastructure.LinkProvider.Abstractions;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Shared.Dtos;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class PaginatedLinkProvider<TItem>
    : LinkProviderBase,
      ICollectionLinkProvider<PagnitatedResponse<TItem>, PagedQueryDto>
{
    public PaginatedLinkProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
        : base(linkGenerator, httpContextAccessor)
    {
    }

    public IReadOnlyCollection<LinkDto> CreateLinks(PagnitatedResponse<TItem> collection, PagedQueryDto query)
    {
        var links = new List<LinkDto>
        {
            CreateLink(
            nameof(GetCourses),
            LinkNames.Self,
            HttpMethods.Get,
            query)
        };

        if (collection.HasNextPage)
        {
            PagedQueryDto nextQuery = query with { Page = query.Page + 1 };
            links.Add(CreateLink(
                nameof(GetCourses),
                LinkNames.Pagination.NextPage,
                HttpMethods.Get,
                nextQuery));
        }

        if (collection.HasPreviousPage)
        {
            PagedQueryDto prevQuery = query with { Page = query.Page - 1 };
            links.Add(CreateLink(
                nameof(GetCourses),
                LinkNames.Pagination.PreviousPage,
                HttpMethods.Get,
                prevQuery));
        }

        return links;
    }
}
