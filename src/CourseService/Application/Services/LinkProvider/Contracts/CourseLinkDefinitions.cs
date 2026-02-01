using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider.Contracts;

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
            new LinkDefinition<CourseState>(
                rel: LinkRels.Self,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetCourseById,
                policyCheck: state => _policy.Can(CourseAction.Read, state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: LinkRels.PartialUpdate,
                method: LinkHttpMethod.Patch,
                endpointName: EndpointNames.PatchCourse,
                policyCheck: state => _policy.Can(CourseAction.Update, state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: LinkRels.Delete,
                method: LinkHttpMethod.Delete,
                endpointName: EndpointNames.DeleteCourse,
                policyCheck: state => _policy.Can(CourseAction.Delete, state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: LinkRels.Course.GenerateImageUploadUrl,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.GenerateCourseImageUploadUrl,
                policyCheck: state => _policy.Can(CourseAction.GenerateImageUploadUrl, state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: LinkRels.Course.CreateModule,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.CreateModule,
                policyCheck: state => _policy.Can(CourseAction.CreateModule, state),
                getRouteValues: state => new { courseId = state.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
