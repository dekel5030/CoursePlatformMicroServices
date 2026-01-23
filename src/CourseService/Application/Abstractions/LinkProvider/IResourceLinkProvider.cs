using Courses.Application.Actions.CourseCollection;
using Courses.Application.Actions.Courses;
using Courses.Application.Actions.Lessons;
using Courses.Application.Actions.Modules;

namespace Courses.Application.Abstractions.LinkProvider;

public interface IResourceLinkProvider<TState>
{
    IReadOnlyCollection<LinkDto> CreateLinks(TState state);
}


public interface ICourseLinkProvider : IResourceLinkProvider<CourseState>;

public interface IModuleLinkProvider : IResourceLinkProvider<ModuleState>;

public interface ILessonLinkProvider : IResourceLinkProvider<LessonState>;
