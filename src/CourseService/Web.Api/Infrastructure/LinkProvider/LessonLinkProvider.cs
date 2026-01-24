using Courses.Api.Endpoints.Lessons;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class LessonLinkProvider : ILessonLinkProvider
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LessonLinkProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public LinkDto GetSelfLink(Guid moduleId, Guid lessonId)
    {
        return CreateLink(
            nameof(GetLessonById),
            LinkNames.Self,
            HttpMethods.Get,
            new { moduleId, lessonId });
    }

    public LinkDto GetUpdateLink(Guid moduleId, Guid lessonId)
    {
        return CreateLink(
            nameof(PatchLesson),
            LinkNames.Update,
            HttpMethods.Patch,
            new { moduleId, lessonId });
    }

    public LinkDto GetDeleteLink(Guid moduleId, Guid lessonId)
    {
        return CreateLink(
            nameof(DeleteLesson),
            LinkNames.Delete,
            HttpMethods.Delete,
            new { moduleId, lessonId });
    }

    public LinkDto GetUploadVideoUrlLink(Guid moduleId, Guid lessonId)
    {
        return CreateLink(
            nameof(GenerateLessonVideoUploadUrl),
            LinkNames.Lessons.UploadVideoUrl,
            HttpMethods.Post,
            new { moduleId, lessonId });
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
