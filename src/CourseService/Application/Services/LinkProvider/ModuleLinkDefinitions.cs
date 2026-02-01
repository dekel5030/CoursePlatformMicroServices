using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Services.LinkProvider;

internal sealed class ModuleLinkDefinitions : ILinkDefinitionRegistry
{
    private readonly CourseGovernancePolicy _policy;
    private IReadOnlyList<ILinkDefinition>? _definitions;

    public ModuleLinkDefinitions(CourseGovernancePolicy policy)
    {
        _policy = policy;
    }

    public LinkResourceKey ResourceKey => LinkResourceKey.Module;

    public IReadOnlyList<ILinkDefinition> GetDefinitions()
    {
        if (_definitions is not null)
        {
            return _definitions;
        }

        _definitions = new List<ILinkDefinition>
        {
            new LinkDefinition<ModuleLinkContext>(
                rel: LinkRels.Module.CreateLesson,
                method: LinkHttpMethod.Post,
                endpointName: EndpointNames.CreateLesson,
                policyCheck: ctx => _policy.Can(ModuleAction.CreateLesson, ctx),
                getRouteValues: ctx => new { moduleId = ctx.ModuleState.Id.Value })
        }.AsReadOnly();

        return _definitions;
    }
}
