using Courses.Application.Actions.Abstract;

namespace Courses.Application.Actions.Courses;

public interface ICourseActionProvider : IActionProvider<CourseAction, CourseState>;
