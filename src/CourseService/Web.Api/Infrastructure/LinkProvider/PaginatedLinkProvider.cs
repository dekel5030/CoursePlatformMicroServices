using CoursePlatform.ServiceDefaults.Dtos;
using Courses.Api.Endpoints.Courses;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Shared.Dtos;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class PaginatedLinkProvider<TItem>
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PaginatedLinkProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
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

    private LinkDto CreateLink(string endpointName, string rel, string method, object? values = null)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        string? href = _linkGenerator.GetUriByName(httpContext, endpointName, values);

        return new LinkDto
        {
            Href = href ?? throw new InvalidOperationException($"Could not generate URL for endpoint '{endpointName}'.kind."),
            Rel = rel,
            Method = method
        };
    }
}
