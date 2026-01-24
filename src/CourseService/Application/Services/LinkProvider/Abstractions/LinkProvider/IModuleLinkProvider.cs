namespace Courses.Application.Services.LinkProvider.Abstractions.LinkProvider;

public interface IModuleLinkProvider
{
    LinkDto GetCreateLessonLink(Guid moduleId);
}
