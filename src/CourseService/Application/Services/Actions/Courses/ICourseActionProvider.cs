using Courses.Application.Services.Actions.Abstractions;

namespace Courses.Application.Services.Actions.Courses;

public interface ICourseActionProvider : IActionProvider<CourseAction, CourseState>;
