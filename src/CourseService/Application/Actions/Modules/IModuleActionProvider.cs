using Courses.Application.Actions.Abstractions;
using Courses.Application.Actions.CourseCollection;

namespace Courses.Application.Actions.Modules;

public interface IModuleActionProvider : IActionProvider<ModuleAction, ModuleState>;
