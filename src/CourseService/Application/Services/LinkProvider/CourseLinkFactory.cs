using Courses.Application.Services.Actions.Courses;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Domain.Courses;

namespace Courses.Application.Services.LinkProvider;

internal sealed class CourseLinkFactory : ICourseLinkFactory
{
    private readonly ICourseActionProvider _courseActionProvider;
    private readonly ICourseLinkProvider _courseLinkService;

    public CourseLinkFactory(
        ICourseActionProvider courseActionProvider,
        ICourseLinkProvider courseLinkService)
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
