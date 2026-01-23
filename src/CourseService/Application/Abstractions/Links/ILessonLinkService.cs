using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Application.Abstractions.Links;

public interface ILessonLinkService
{
    LinkDto GetSelfLink(Guid moduleId, Guid lessonId);
    LinkDto GetUpdateLink(Guid moduleId, Guid lessonId);
    LinkDto GetDeleteLink(Guid moduleId, Guid lessonId);
    LinkDto GetUploadVideoUrlLink(Guid moduleId, Guid lessonId);
}
