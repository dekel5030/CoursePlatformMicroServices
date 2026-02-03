using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

internal sealed class CourseLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public CourseLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public LinkResourceKey ResourceKey => LinkResourceKey.Course;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<CourseContext>(
                rel: LinkRels.Self,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourseById,
                policyCheck: ctx => _policy.CanReadCourse(ctx),
                getRouteValues: ctx => new { id = ctx.Id.Value }),

            new LinkDefinition<CourseContext>(
                rel: LinkRels.PartialUpdate,
                method: LinkHttpMethod.Patch,
                endpointName: EndpointNames.PatchCourse,
                policyCheck: ctx => _policy.CanEditCourse(ctx),
                getRouteValues: ctx => new { id = ctx.Id.Value }),

            new LinkDefinition<CourseContext>(
                rel: LinkRels.Delete,
                method: LinkHttpMethod.Delete,
                endpointName: EndpointNames.DeleteCourse,
                policyCheck: ctx => _policy.CanDeleteCourse(ctx),
                getRouteValues: ctx => new { id = ctx.Id.Value }),

            new LinkDefinition<CourseContext>(
                rel: LinkRels.Course.GenerateImageUploadUrl,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.GenerateCourseImageUploadUrl,
                policyCheck: ctx => _policy.CanEditCourse(ctx),
                getRouteValues: ctx => new { Id = ctx.Id.Value }),

            new LinkDefinition<CourseContext>(
                rel: LinkRels.CourseRating.Ratings,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourseRatings,
                policyCheck: _ => true,
                getRouteValues: ctx => new { courseId = ctx.Id.Value }),

            new LinkDefinition<CourseContext>(
                rel: LinkRels.Course.CreateModule,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.CreateModule,
                policyCheck: ctx => _policy.CanEditCourse(ctx),
                getRouteValues: ctx => new { courseId = ctx.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
