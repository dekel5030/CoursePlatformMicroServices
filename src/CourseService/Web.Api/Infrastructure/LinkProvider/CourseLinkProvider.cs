using Courses.Api.Endpoints.Courses;
using Courses.Api.Endpoints.Modules;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;
using Courses.Application.Shared.Dtos;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class CourseLinkProvider : ICourseLinkProvider
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CourseLinkProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    private LinkDto CreateLink(string endpointName, string rel, string method, object? values = null)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        string? href = _linkGenerator.GetUriByName(httpContext, endpointName, values);

        return new LinkDto
        {
            Href = href ?? throw new InvalidOperationException($"Could not generate URL for endpoint '{endpointName}'."),
            Rel = rel,
            Method = method
        };
    }

    public LinkDto GetSelfLink(Guid courseId)
    {
        return CreateLink(nameof(GetCourseById), LinkNames.Self, HttpMethods.Get, new { id = courseId });
    }

    public LinkDto GetUpdateLink(Guid courseId)
    {
        return CreateLink(nameof(PatchCourse), LinkNames.Update, HttpMethods.Patch, new { id = courseId });
    }

    public LinkDto GetDeleteLink(Guid courseId)
    {
        return CreateLink(nameof(DeleteCourse), LinkNames.Delete, HttpMethods.Delete, new { id = courseId });
    }

    public LinkDto GetUploadImageUrlLink(Guid courseId)
    {
        return CreateLink(
            nameof(GenerateCourseImageUploadUrl),
            LinkNames.Courses.GenerateImageUploadUrl,
            HttpMethods.Post,
            new { id = courseId });
    }

    public LinkDto GetCreateModuleLink(Guid courseId)
    {
        return CreateLink(
            nameof(CreateModule),
            LinkNames.Courses.CreateModule,
            HttpMethods.Post,
            new { courseId });
    }

    public List<LinkDto> GetPaginationLinks(PagedQueryDto query, int totalCount)
    {
        var links = new List<LinkDto>
        {
            CreateLink(nameof(GetCourses), LinkNames.Self, HttpMethods.Get, query)
        };

        if (query.Page * query.PageSize < totalCount)
        {
            links.Add(CreateLink(
                nameof(GetCourses),
                LinkNames.Pagination.NextPage,
                HttpMethods.Get,
                query with { Page = query.Page + 1 }));
        }

        if (query.Page > 1)
        {
            links.Add(CreateLink(
                nameof(GetCourses),
                LinkNames.Pagination.PreviousPage,
                HttpMethods.Get,
                query with { Page = query.Page - 1 }));
        }

        return links;
    }

    public LinkDto GetCreateCourseLink()
    {
        return CreateLink(
            nameof(CreateCourse),
            LinkNames.Create,
            HttpMethods.Post);
    }
}
