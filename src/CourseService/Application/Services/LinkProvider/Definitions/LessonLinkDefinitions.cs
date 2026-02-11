using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.constants;

namespace Courses.Application.Services.LinkProvider.Definitions;

internal sealed class LessonLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public LessonLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public LinkResourceKey ResourceKey => LinkResourceKey.Lesson;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<LessonContext>(
                rel: LinkRels.Self,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetLessonById,
                policyCheck: ctx => _policy.CanReadLesson(ctx),
                getRouteValues: ctx => new { lessonId = ctx.Id.Value }),

            new LinkDefinition<LessonContext>(
                rel: LinkRels.PartialUpdate,
                method: LinkHttpMethod.Patch,
                endpointName: EndpointNames.PatchLesson,
                policyCheck: ctx => ctx.Module.Course.IsManagementView && _policy.CanEditLesson(ctx),
                getRouteValues: ctx => new { lessonId = ctx.Id.Value }),

            new LinkDefinition<LessonContext>(
                rel: LinkRels.Lesson.UploadVideoUrl,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.GenerateLessonVideoUploadUrl,
                policyCheck: ctx => ctx.Module.Course.IsManagementView && _policy.CanEditLesson(ctx),
                getRouteValues: ctx => new { lessonId = ctx.Id.Value }),

            new LinkDefinition<LessonContext>(
                rel: LinkRels.Lesson.AiGenerate,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.GenerateLessonWithAi,
                policyCheck: ctx => ctx.Module.Course.IsManagementView && _policy.CanEditLesson(ctx),
                getRouteValues: ctx => new { lessonId = ctx.Id.Value }),

            new LinkDefinition<LessonContext>(
                rel: LinkRels.Lesson.Move,
                method: LinkHttpMethod.Patch,
                endpointName: EndpointNames.MoveLesson,
                policyCheck: ctx => ctx.Module.Course.IsManagementView && _policy.CanEditLesson(ctx),
                getRouteValues: ctx => new { lessonId = ctx.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
