using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider;

internal sealed class LinkProvider : ILinkProvider
{
    private readonly IHttpLinkResolver _resolver;

    public LinkProvider(IHttpLinkResolver resolver)
    {
        _resolver = resolver;
    }

    private static LinkRecord Link(string href, string method) => new(href, method);

    public LinkRecord GetManagedCourseLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.GetManagedCourseById, new { id = courseId }), LinkHttpMethod.Get);

    public LinkRecord GetCoursePageLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.GetCoursePage, new { id = courseId }), LinkHttpMethod.Get);

    public LinkRecord GetCourseAnalyticsLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.GetCourseAnalytics, new { id = courseId }), LinkHttpMethod.Get);

    public LinkRecord GetPatchCourseLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.PatchCourse, new { id = courseId }), LinkHttpMethod.Patch);

    public LinkRecord GetDeleteCourseLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.DeleteCourse, new { id = courseId }), LinkHttpMethod.Delete);

    public LinkRecord GetPublishCourseLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.PublishCourse, new { id = courseId }), LinkHttpMethod.Post);

    public LinkRecord GetGenerateCourseImageUploadUrlLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.GenerateCourseImageUploadUrl, new { id = courseId }), LinkHttpMethod.Post);

    public LinkRecord GetCreateModuleLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.CreateModule, new { courseId }), LinkHttpMethod.Post);

    public LinkRecord GetReorderModulesLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.ReorderModules, new { courseId }), LinkHttpMethod.Patch);

    public LinkRecord GetCourseRatingsLink(Guid courseId) =>
        Link(_resolver.GetHref(EndpointNames.GetCourseRatings, new { courseId }), LinkHttpMethod.Get);

    public LinkRecord GetCreateLessonLink(Guid moduleId) =>
        Link(_resolver.GetHref(EndpointNames.CreateLesson, new { moduleId }), LinkHttpMethod.Post);

    public LinkRecord GetPatchModuleLink(Guid moduleId) =>
        Link(_resolver.GetHref(EndpointNames.PatchModule, new { moduleId }), LinkHttpMethod.Patch);

    public LinkRecord GetDeleteModuleLink(Guid moduleId) =>
        Link(_resolver.GetHref(EndpointNames.DeleteModule, new { moduleId }), LinkHttpMethod.Delete);

    public LinkRecord GetReorderLessonsLink(Guid moduleId) =>
        Link(_resolver.GetHref(EndpointNames.ReorderLessons, new { moduleId }), LinkHttpMethod.Patch);

    public LinkRecord GetLessonPageLink(Guid lessonId) =>
        Link(_resolver.GetHref(EndpointNames.GetLessonPage, new { lessonId }), LinkHttpMethod.Get);

    public LinkRecord GetPatchLessonLink(Guid lessonId) =>
        Link(_resolver.GetHref(EndpointNames.PatchLesson, new { lessonId }), LinkHttpMethod.Patch);

    public LinkRecord GetLessonVideoUploadUrlLink(Guid lessonId) =>
        Link(_resolver.GetHref(EndpointNames.GenerateLessonVideoUploadUrl, new { lessonId }), LinkHttpMethod.Post);

    public LinkRecord GetGenerateLessonWithAiLink(Guid lessonId) =>
        Link(_resolver.GetHref(EndpointNames.GenerateLessonWithAi, new { lessonId }), LinkHttpMethod.Post);

    public LinkRecord GetMoveLessonLink(Guid lessonId) =>
        Link(_resolver.GetHref(EndpointNames.MoveLesson, new { lessonId }), LinkHttpMethod.Patch);
}
