using Courses.Application.Services.Actions.States;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

internal interface IModuleLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(CourseState courseState, ModuleState moduleState);
}
