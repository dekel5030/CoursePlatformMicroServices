using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;

namespace Courses.Application.Services.LinkProvider;

internal sealed class LessonLinkFactory : ILessonLinkFactory
{
    private readonly CourseGovernancePolicy _policy;
    private readonly ILessonLinkProvider _lessonLinkService;

    public LessonLinkFactory(
        CourseGovernancePolicy policy,
        ILessonLinkProvider lessonLinkService)
    {
        _policy = policy;
        _lessonLinkService = lessonLinkService;
    }

    public IReadOnlyList<LinkDto> CreateLinks(
        CourseState courseState,
        ModuleState moduleState,
        LessonState lessonState,
        EnrollmentState? enrollmentState = null)
    {
        var links = new List<LinkDto>();
        Guid moduleId = moduleState.Id.Value;
        Guid lessonId = lessonState.Id.Value;

        if (_policy.CanReadLesson(courseState, lessonState, enrollmentState))
        {
            links.Add(_lessonLinkService.GetSelfLink(moduleId, lessonId));
        }

        if (_policy.CanEditLesson(courseState))
        {
            links.Add(_lessonLinkService.GetUpdateLink(moduleId, lessonId));
            links.Add(_lessonLinkService.GetDeleteLink(moduleId, lessonId));
            links.Add(_lessonLinkService.GetUploadVideoUrlLink(moduleId, lessonId));
        }

        return links.AsReadOnly();
    }
}
