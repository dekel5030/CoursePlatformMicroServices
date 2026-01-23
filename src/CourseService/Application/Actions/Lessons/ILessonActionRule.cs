using Courses.Application.Actions.Abstractions;

namespace Courses.Application.Actions.Lessons;

public interface ILessonActionRule : IActionRule<LessonAction, LessonState>;
