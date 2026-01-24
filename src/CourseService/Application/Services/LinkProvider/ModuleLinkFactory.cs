using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;

namespace Courses.Application.Services.LinkProvider;

internal sealed class ModuleLinkFactory : IModuleLinkFactory
{
    private readonly CourseGovernancePolicy _policy;
    private readonly IModuleLinkProvider _moduleLinkService;

    public ModuleLinkFactory(
        CourseGovernancePolicy policy,
        IModuleLinkProvider moduleLinkService)
    {
        _policy = policy;
        _moduleLinkService = moduleLinkService;
    }

    public IReadOnlyList<LinkDto> CreateLinks(CourseState courseState, ModuleState moduleState)
    {
        var links = new List<LinkDto>();

        if (_policy.CanEditModule(courseState))
        {
            links.Add(_moduleLinkService.GetCreateLessonLink(moduleState.Id.Value));
        }

        return links.AsReadOnly();
    }
}
