using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider;

internal sealed class CourseLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public CourseLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public string ResourceKey => LinkResourceKeys.Course;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<CourseState>(
                rel: "self",
                method: "GET",
                endpointName: "GetCourseById",
                policyCheck: state => _policy.CanReadCourse(state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: "partial-update",
                method: "PATCH",
                endpointName: "PatchCourse",
                policyCheck: state => _policy.CanEditCourseContent(state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: "delete",
                method: "DELETE",
                endpointName: "DeleteCourse",
                policyCheck: state => _policy.CanDeleteCourse(state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: "generate-image-upload-url",
                method: "POST",
                endpointName: "GenerateCourseImageUploadUrl",
                policyCheck: state => _policy.CanEditCourseContent(state),
                getRouteValues: state => new { id = state.Id.Value }),

            new LinkDefinition<CourseState>(
                rel: "create-module",
                method: "POST",
                endpointName: "CreateModule",
                policyCheck: state => _policy.CanEditCourseContent(state),
                getRouteValues: state => new { courseId = state.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
