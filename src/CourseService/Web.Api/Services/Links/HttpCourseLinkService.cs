using Courses.Api.Endpoints.Courses;
using Courses.Api.Endpoints.Modules;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Abstractions.Links;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Services.Links;

internal sealed class HttpCourseLinkService : ICourseLinkService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCourseLinkService(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
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
}
