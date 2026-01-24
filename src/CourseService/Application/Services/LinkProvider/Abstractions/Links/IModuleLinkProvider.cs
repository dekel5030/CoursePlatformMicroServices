namespace Courses.Application.Services.LinkProvider.Abstractions.Links;

public interface IModuleLinkProvider
{
    LinkDto GetCreateLessonLink(Guid moduleId);
}
