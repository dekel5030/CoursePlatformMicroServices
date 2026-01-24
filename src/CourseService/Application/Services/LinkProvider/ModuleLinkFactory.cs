using Courses.Application.Services.Actions.Modules;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Domain.Module;

namespace Courses.Application.Services.LinkProvider;

internal sealed class ModuleLinkFactory : IModuleLinkFactory
{
    private readonly IModuleActionProvider _moduleActionProvider;
    private readonly IModuleLinkProvider _moduleLinkService;

    public ModuleLinkFactory(
        IModuleActionProvider moduleActionProvider,
        IModuleLinkProvider moduleLinkService)
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
