using Courses.Api.Endpoints.Courses;
using Courses.Api.Endpoints.Lessons;
using Courses.Application.Actions.Primitives;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class LinkProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public LinkProvider(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public LinkDto Create(string endpointName, string rel, string method, object? values = null)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        string? href = _linkGenerator.GetUriByName(
            httpContext,
            endpointName,
            values);

        return new LinkDto
        {
            Href = href ?? throw new InvalidOperationException($"Could not generate URL for endpoint '{endpointName}'."),
            Rel = rel,
            Method = method
        };
    }

    public List<LinkDto> CreateCourseLinks(CourseId id, IReadOnlyCollection<CourseAction> allowedActions)
    {
        var allowedActionsSet = allowedActions.ToHashSet();
        var links = new List<LinkDto>();
        string idStr = id.Value.ToString();

        links.Add(Create(nameof(GetCourseById), "self", HttpMethods.Get, new { id = idStr }));

        if (allowedActionsSet.TryGetValue(CourseAction.Update, out _))
        {
            links.Add(Create(nameof(PatchCourse), "partial-update", HttpMethods.Patch, new { id = idStr }));
        }

        if (allowedActionsSet.TryGetValue(CourseAction.Delete, out _))
        {
            links.Add(Create(nameof(DeleteCourse), "delete", HttpMethods.Delete, new { id = idStr }));
        }

        if (allowedActionsSet.TryGetValue(CourseAction.CreateLesson, out _))
        {
            links.Add(Create(nameof(CreateLesson), "create-lesson", HttpMethods.Post, new { courseId = idStr }));
        }

        if (allowedActionsSet.TryGetValue(CourseAction.UploadImageUrl, out _))
        {
            links.Add(Create(nameof(GenerateCourseImageUploadUrl), "upload-image", HttpMethods.Post, new { id = idStr }));
        }

        return links;
    }

    public List<LinkDto> CreateLessonLinks(
        CourseId courseId,
        LessonId lessonId,
        IReadOnlyCollection<LessonAction> allowedActions)
    {
        var allowedActionsSet = allowedActions.ToHashSet();
        var links = new List<LinkDto>();
        string courseIdStr = courseId.Value.ToString();
        string lessonIdStr = lessonId.Value.ToString();
        links.Add(Create(nameof(GetLessonById), "self", HttpMethods.Get, new { courseId = courseIdStr, lessonId = lessonIdStr }));

        if (allowedActionsSet.TryGetValue(LessonAction.Update, out _))
        {
            links.Add(Create(nameof(PatchLesson), "partial-update", HttpMethods.Patch, new { courseId = courseIdStr, lessonId = lessonIdStr }));
        }

        if (allowedActionsSet.TryGetValue(LessonAction.Delete, out _))
        {
            links.Add(Create(nameof(DeleteLesson), "delete", HttpMethods.Delete, new { courseId = courseIdStr, lessonId = lessonIdStr }));
        }

        if (allowedActionsSet.TryGetValue(LessonAction.Create, out _))
        {
            links.Add(Create(nameof(CreateLesson), "create", HttpMethods.Post, new { courseId = courseIdStr }));
        }

        return links;
    }

    public List<LinkDto> CreateCourseCollectionLinks(
        CourseCollectionDto courseCollection,
        PagedQueryDto originalQuery)
    {
        List<LinkDto> links = CreatePagedLinks(courseCollection, originalQuery);
        var allowedActionsSet = courseCollection.AllowedActions.ToHashSet();

        if (allowedActionsSet.TryGetValue(CourseCollectionAction.CreateCourse, out _))
        {
            links.Add(Create(nameof(CreateCourse), "create", HttpMethods.Post));
        }

        return links;
    }

    private List<LinkDto> CreatePagedLinks(
        CourseCollectionDto courseCollection,
        object originalQuery)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        string? endpointName = httpContext.GetEndpoint()?.Metadata
            .GetMetadata<EndpointNameMetadata>()?.EndpointName;

        if (string.IsNullOrEmpty(endpointName))
        {
            throw new InvalidOperationException("Endpoint name metadata is missing.");
        }

        var links = new List<LinkDto>();
        var routeValues = new RouteValueDictionary(originalQuery);

        links.Add(Create(endpointName, "self", HttpMethods.Get, routeValues));

        if (courseCollection.HasNextPage)
        {
            var nextValues = new RouteValueDictionary(originalQuery)
            {
                [nameof(PagedQueryDto.PageNumber)] = courseCollection.PageNumber + 1
            };
            links.Add(Create(endpointName, "next-page", HttpMethods.Get, nextValues));
        }

        if (courseCollection.HasPreviousPage)
        {
            var prevValues = new RouteValueDictionary(originalQuery)
            {
                [nameof(PagedQueryDto.PageNumber)] = courseCollection.PageNumber - 1
            };
            links.Add(Create(endpointName, "previous-page", HttpMethods.Get, prevValues));
        }

        return links;
    }
}
