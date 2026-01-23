using Courses.Application.Actions.Abstractions;

namespace Courses.Application.Actions.Courses;

public interface ICourseActionProvider : IActionProvider<CourseAction, CourseState>;
