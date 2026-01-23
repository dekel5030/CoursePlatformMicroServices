using Courses.Api.Endpoints.Courses;
using Courses.Api.Endpoints.Modules;
using Courses.Api.Infrastructure.LinkProvider.Abstractions;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Actions.Courses;
using Courses.Application.Courses.Dtos;

namespace Courses.Api.Infrastructure.LinkProvider;

internal sealed class CourseLinkProvider : LinkProviderBase, ICourseLinkProvider
{
    private readonly ICourseActionProvider _courseActionProvider;

    public CourseLinkProvider(
        LinkGenerator linkGenerator, 
        IHttpContextAccessor httpContextAccessor, 
        ICourseActionProvider courseActionProvider) : base(linkGenerator, httpContextAccessor)
    {
        _courseActionProvider = courseActionProvider;
    }

    public IReadOnlyCollection<LinkDto> CreateLinks(CourseState state)
    {
        var allowed = _courseActionProvider.GetAllowedActions(state).ToHashSet();
        var links = new List<LinkDto>();
        var idObj = new { id = state.CourseId };

        if (allowed.Contains(CourseAction.Read))
        {
            links.Add(CreateLink(nameof(GetCourseById), LinkNames.Self, HttpMethods.Get, idObj));
        }

        if (allowed.Contains(CourseAction.Update))
        {
            links.Add(CreateLink(nameof(PatchCourse), LinkNames.Update, HttpMethods.Patch, idObj));
        }

        if (allowed.Contains(CourseAction.Delete))
        {
            links.Add(CreateLink(nameof(DeleteCourse), LinkNames.Delete, HttpMethods.Delete, idObj ));
        }

        if (allowed.Contains(CourseAction.UploadImageUrl))
        {
            links.Add(CreateLink(
                nameof(GenerateCourseImageUploadUrl), 
                LinkNames.Courses.GenerateImageUploadUrl, HttpMethods.Post, idObj));
        }

        if (allowed.Contains(CourseAction.CreateModule))
        {
            links.Add(CreateLink(
                nameof(CreateModule), 
                LinkNames.Courses.CreateModule, 
                HttpMethods.Post, 
                new { courseId = state.CourseId }));
        }

        return links;
    }
}
