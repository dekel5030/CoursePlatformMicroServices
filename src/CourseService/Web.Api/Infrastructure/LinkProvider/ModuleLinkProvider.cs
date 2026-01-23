using Courses.Api.Endpoints.Lessons;
using Courses.Api.Endpoints.Modules;
using Courses.Api.Infrastructure.LinkProvider.Abstractions;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Actions.Modules;
using Courses.Application.Shared.Dtos;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class ModuleLinkProvider : LinkProviderBase, IModuleLinkProvider
{
    private readonly IModuleActionProvider _moduleActionProvider;

    public ModuleLinkProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        IModuleActionProvider moduleActionProvider) : base(linkGenerator, httpContextAccessor)
    {
        _moduleActionProvider = moduleActionProvider;
    }

    public IReadOnlyCollection<LinkDto> CreateLinks(ModuleState state)
    {
        var allowed = _moduleActionProvider.GetAllowedActions(state).ToHashSet();
        var links = new List<LinkDto>();

        //if (allowed.Contains(ModuleAction.Read))
        //{
        //    links.Add(CreateLink(nameof(
        //        GetModulesByCourseId), 
        //        LinkNames.Self, 
        //        HttpMethods.Get, 
        //        new { courseId = state.CourseId.Value}));
        //}

        if (allowed.Contains(ModuleAction.CreateLesson))
        {
            links.Add(
                CreateLink(nameof(CreateLesson), 
                LinkNames.Modules.CreateLesson, 
                HttpMethods.Post, 
                new { moduleId = state.ModuleId.Value}));
        }

        return links;
    }
}
