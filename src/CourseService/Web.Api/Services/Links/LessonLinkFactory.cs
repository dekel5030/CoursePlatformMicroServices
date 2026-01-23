using Courses.Application.Abstractions.Links;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Actions.Lessons;
using Courses.Domain.Lessons;

namespace Courses.Api.Services.Links;

internal sealed class LessonLinkFactory : ILessonLinkFactory
{
    private readonly ILessonActionProvider _lessonActionProvider;
    private readonly ILessonLinkService _lessonLinkService;

    public LessonLinkFactory(
        ILessonActionProvider lessonActionProvider,
        ILessonLinkService lessonLinkService)
    {
        _lessonActionProvider = lessonActionProvider;
        _lessonLinkService = lessonLinkService;
    }

    public IReadOnlyList<LinkDto> CreateLinks(Lesson lesson)
    {
        var lessonState = new LessonState(lesson.Id);
        var allowedActions = _lessonActionProvider.GetAllowedActions(lessonState).ToHashSet();
        var links = new List<LinkDto>();

        if (allowedActions.Contains(LessonAction.Read))
        {
            links.Add(_lessonLinkService.GetSelfLink(lesson.ModuleId.Value, lesson.Id.Value));
        }

        if (allowedActions.Contains(LessonAction.Update))
        {
            links.Add(_lessonLinkService.GetUpdateLink(lesson.ModuleId.Value, lesson.Id.Value));
        }

        if (allowedActions.Contains(LessonAction.Delete))
        {
            links.Add(_lessonLinkService.GetDeleteLink(lesson.ModuleId.Value, lesson.Id.Value));
        }

        if (allowedActions.Contains(LessonAction.UploadVideoUrl))
        {
            links.Add(_lessonLinkService.GetUploadVideoUrlLink(lesson.ModuleId.Value, lesson.Id.Value));
        }

        return links.AsReadOnly();
    }
}
