namespace Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;

public interface ILessonLinkProvider
{
    LinkDto GetSelfLink(Guid moduleId, Guid lessonId);
    LinkDto GetUpdateLink(Guid moduleId, Guid lessonId);
    LinkDto GetDeleteLink(Guid moduleId, Guid lessonId);
    LinkDto GetUploadVideoUrlLink(Guid moduleId, Guid lessonId);
    LinkDto GetAiGenerateLink(Guid moduleId, Guid lessonId);
}
