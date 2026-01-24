using Courses.Application.Services.Actions.States;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

internal interface ICourseLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(CourseState state);
}
