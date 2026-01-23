using Courses.Application.Abstractions.Links;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Actions.Courses;
using Courses.Domain.Courses;

namespace Courses.Api.Services.Links;

internal sealed class CourseLinkFactory : ICourseLinkFactory
{
    private readonly ICourseActionProvider _courseActionProvider;
    private readonly ICourseLinkService _courseLinkService;

    public CourseLinkFactory(
        ICourseActionProvider courseActionProvider,
        ICourseLinkService courseLinkService)
    {
        _courseActionProvider = courseActionProvider;
        _courseLinkService = courseLinkService;
    }

    public IReadOnlyList<LinkDto> CreateLinks(Course course)
    {
        var courseState = new CourseState(course.Id, course.InstructorId, course.Status, course.LessonCount);
        var allowedActions = _courseActionProvider.GetAllowedActions(courseState).ToHashSet();
        var links = new List<LinkDto>();

        if (allowedActions.Contains(CourseAction.Read))
        {
            links.Add(_courseLinkService.GetSelfLink(course.Id.Value));
        }

        if (allowedActions.Contains(CourseAction.Update))
        {
            links.Add(_courseLinkService.GetUpdateLink(course.Id.Value));
        }

        if (allowedActions.Contains(CourseAction.Delete))
        {
            links.Add(_courseLinkService.GetDeleteLink(course.Id.Value));
        }

        if (allowedActions.Contains(CourseAction.UploadImageUrl))
        {
            links.Add(_courseLinkService.GetUploadImageUrlLink(course.Id.Value));
        }

        if (allowedActions.Contains(CourseAction.CreateModule))
        {
            links.Add(_courseLinkService.GetCreateModuleLink(course.Id.Value));
        }

        return links.AsReadOnly();
    }
}
