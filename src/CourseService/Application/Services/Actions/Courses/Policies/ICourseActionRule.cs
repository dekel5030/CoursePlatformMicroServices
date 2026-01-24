using Courses.Application.Services.Actions.Abstractions;

namespace Courses.Application.Services.Actions.Courses.Policies;

public interface ICourseActionRule : IActionRule<CourseAction, CourseState>;
