using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider.Contracts;

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
            new LinkDefinition<LessonLinkContext>(
                rel: LinkRels.Self,
                method: LinkHttpMethod.Get,
                endpointName: EndpointNames.GetLessonById,
                policyCheck: ctx => _policy.Can(LessonAction.Read, ctx),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: LinkRels.PartialUpdate,
                method: LinkHttpMethod.Patch,
                endpointName: EndpointNames.PatchLesson,
                policyCheck: ctx => _policy.Can(LessonAction.Update, ctx),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: LinkRels.Delete,
                method: LinkHttpMethod.Delete,
                endpointName: EndpointNames.DeleteLesson,
                policyCheck: ctx => _policy.Can(LessonAction.Delete, ctx),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: LinkRels.Lesson.UploadVideoUrl,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.GenerateLessonVideoUploadUrl,
                policyCheck: ctx => _policy.Can(LessonAction.UploadVideoUrl, ctx),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: LinkRels.Lesson.AiGenerate,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.GenerateLessonWithAi,
                policyCheck: ctx => _policy.Can(LessonAction.AiGenerate, ctx),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
