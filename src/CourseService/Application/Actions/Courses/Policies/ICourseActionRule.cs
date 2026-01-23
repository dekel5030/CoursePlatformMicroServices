using Courses.Application.Actions.Abstract;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Courses.Policies;

public interface ICourseActionRule : IActionRule<CourseAction, CourseState>;
