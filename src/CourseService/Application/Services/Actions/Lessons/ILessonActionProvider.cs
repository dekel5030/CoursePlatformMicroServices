using Courses.Application.Services.Actions.Abstractions;

namespace Courses.Application.Services.Actions.Lessons;

public interface ILessonActionProvider : IActionProvider<LessonAction, LessonState>;
