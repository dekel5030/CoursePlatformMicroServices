using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

internal sealed class EnrolledCourseLinkDefinitions : ILinkDefinitionRegistry
{
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public LinkResourceKey ResourceKey => LinkResourceKey.EnrolledCourse;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<EnrolledCourseContext>(
                rel: LinkRels.EnrolledCourse.ViewCourse,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourseById,
                policyCheck: _ => true,
                getRouteValues: ctx => new { id = ctx.CourseId.Value }),

            new LinkDefinition<EnrolledCourseContext>(
                rel: LinkRels.EnrolledCourse.ContinueLearning,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetLessonById,
                policyCheck: ctx => ctx.LastAccessedLessonId is not null,
                getRouteValues: ctx => new { lessonId = ctx.LastAccessedLessonId!.Value }),

            new LinkDefinition<EnrolledCourseContext>(
                rel: LinkRels.EnrolledCourse.ContinueLearning,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourseById,
                policyCheck: ctx => ctx.LastAccessedLessonId is null,
                getRouteValues: ctx => new { id = ctx.CourseId.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
