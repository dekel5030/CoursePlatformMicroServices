using Courses.Application.Actions.Abstractions;

namespace Courses.Application.Actions.Lessons;

public interface ILessonActionProvider : IActionProvider<LessonAction, LessonState>;
