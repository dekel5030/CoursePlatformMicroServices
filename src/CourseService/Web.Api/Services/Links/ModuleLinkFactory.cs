using Courses.Application.Abstractions.Links;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Actions.Modules;
using Courses.Domain.Module;

namespace Courses.Api.Services.Links;

internal sealed class ModuleLinkFactory : IModuleLinkFactory
{
    private readonly IModuleActionProvider _moduleActionProvider;
    private readonly IModuleLinkService _moduleLinkService;

    public ModuleLinkFactory(
        IModuleActionProvider moduleActionProvider,
        IModuleLinkService moduleLinkService)
    {
        _moduleActionProvider = moduleActionProvider;
        _moduleLinkService = moduleLinkService;
    }

    public IReadOnlyList<LinkDto> CreateLinks(Module module)
    {
        var moduleState = new ModuleState(module.CourseId, module.Id);
        var allowedActions = _moduleActionProvider.GetAllowedActions(moduleState).ToHashSet();
        var links = new List<LinkDto>();

        if (allowedActions.Contains(ModuleAction.CreateLesson))
        {
            links.Add(_moduleLinkService.GetCreateLessonLink(module.Id.Value));
        }

        return links.AsReadOnly();
    }
}
