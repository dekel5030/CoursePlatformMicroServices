using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Application.Abstractions.Links;

public interface IModuleLinkService
{
    LinkDto GetCreateLessonLink(Guid moduleId);
}
