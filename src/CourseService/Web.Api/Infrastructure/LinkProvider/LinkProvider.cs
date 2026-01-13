using Courses.Api.Endpoints.Courses;
using Courses.Api.Endpoints.Lessons;
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

    public List<LinkDto> CreateCourseLinks(CourseId id)
    {
        string idStr = id.Value.ToString();
        return new List<LinkDto>
        {
            Create(nameof(GetCourseById), "self", HttpMethods.Get, new { id = idStr }),
            Create(nameof(PatchCourse), "partial-update", HttpMethods.Post, new { id = idStr }),
            Create(nameof(DeleteCourse), "delete", HttpMethods.Delete, new { id = idStr })
        };
    }

    public List<LinkDto> CreateLessonLinks(CourseId courseId, LessonId lessonId)
    {
        string courseIdStr = courseId.Value.ToString();
        string lessonIdStr = lessonId.Value.ToString();
        return new List<LinkDto>
        {
            Create(nameof(GetLessonById), "self", HttpMethods.Get, new { courseId = courseIdStr, lessonId = lessonIdStr }),
            Create(nameof(PatchLesson), "partial-update", HttpMethods.Post, new { courseId = courseIdStr, lessonId = lessonIdStr }),
            Create(nameof(DeleteLesson), "delete", HttpMethods.Delete, new { courseId = courseIdStr, lessonId = lessonIdStr })
        };
    }
}