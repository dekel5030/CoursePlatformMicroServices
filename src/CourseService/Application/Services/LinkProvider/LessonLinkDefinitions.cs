using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider;

internal sealed class LessonLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public LessonLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public string ResourceKey => LinkResourceKeys.Lesson;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<LessonLinkContext>(
                rel: "self",
                method: "GET",
                endpointName: "GetLessonById",
                policyCheck: ctx => _policy.CanReadLesson(ctx.CourseState, ctx.LessonState, ctx.EnrollmentState),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: "partial-update",
                method: "PATCH",
                endpointName: "PatchLesson",
                policyCheck: ctx => _policy.CanEditLesson(ctx.CourseState),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: "delete",
                method: "DELETE",
                endpointName: "DeleteLesson",
                policyCheck: ctx => _policy.CanEditLesson(ctx.CourseState),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: "upload-video-url",
                method: "POST",
                endpointName: "GenerateLessonVideoUploadUrl",
                policyCheck: ctx => _policy.CanEditLesson(ctx.CourseState),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value }),

            new LinkDefinition<LessonLinkContext>(
                rel: "ai-generate",
                method: "POST",
                endpointName: "GenerateLessonWithAi",
                policyCheck: ctx => _policy.CanEditLesson(ctx.CourseState),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value, lessonId = ctx.LessonState.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
