using Courses.Application.Services.Actions.States;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

internal interface ICourseLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(CourseState state);
    IReadOnlyList<LinkDto> CreateCollectionLinks(PagedQueryDto query, int totalCount);
}
