namespace Courses.Application.Services.LinkProvider.Abstractions.Links;

public interface ILessonLinkProvider
{
    LinkDto GetSelfLink(Guid moduleId, Guid lessonId);
    LinkDto GetUpdateLink(Guid moduleId, Guid lessonId);
    LinkDto GetDeleteLink(Guid moduleId, Guid lessonId);
    LinkDto GetUploadVideoUrlLink(Guid moduleId, Guid lessonId);
}
