using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Services.LinkProvider;

internal sealed class CourseLinkFactory : ICourseLinkFactory
{
    private readonly CourseGovernancePolicy _policy;
    private readonly ICourseLinkProvider _courseLinkService;

    public CourseLinkFactory(
        CourseGovernancePolicy policy,
        ICourseLinkProvider courseLinkService)
    {
        _policy = policy;
        _courseLinkService = courseLinkService;
    }

    public IReadOnlyList<LinkDto> CreateLinks(CourseState state)
    {
        var links = new List<LinkDto>();
        Guid courseId = state.Id.Value;

        if (_policy.CanReadCourse(state))
        {
            links.Add(_courseLinkService.GetSelfLink(courseId));
        }

        if (_policy.CanEditCourseContent(state))
        {
            links.Add(_courseLinkService.GetUpdateLink(courseId));
            links.Add(_courseLinkService.GetUploadImageUrlLink(courseId));
            links.Add(_courseLinkService.GetCreateModuleLink(courseId));
        }

        if (_policy.CanDeleteCourse(state))
        {
            links.Add(_courseLinkService.GetDeleteLink(courseId));
        }

        return links.AsReadOnly();
    }

    public IReadOnlyList<LinkDto> CreateCollectionLinks(PagedQueryDto query, int totalCount)
    {
        List<LinkDto> links = _courseLinkService.GetPaginationLinks(query, totalCount);

        if (_policy.CanCreateCourse())
        {
            links.Add(_courseLinkService.GetCreateCourseLink());
        }

        return links.AsReadOnly();
    }
}
