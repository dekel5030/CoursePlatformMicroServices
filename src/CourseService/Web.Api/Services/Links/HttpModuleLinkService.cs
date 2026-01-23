using Courses.Api.Endpoints.Lessons;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Abstractions.Links;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Services.Links;

internal sealed class HttpModuleLinkService : IModuleLinkService
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpModuleLinkService(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public LinkDto GetCreateLessonLink(Guid moduleId)
    {
        return CreateLink(
            nameof(CreateLesson),
            LinkNames.Modules.CreateLesson,
            HttpMethods.Post,
            new { moduleId });
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
