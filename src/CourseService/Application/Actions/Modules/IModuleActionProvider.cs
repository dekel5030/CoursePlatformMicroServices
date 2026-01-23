using Courses.Application.Actions.Abstract;
using Courses.Application.Actions.CourseCollection;
using Courses.Application.Actions.Modules.Policies;

namespace Courses.Application.Actions.Modules;

public interface IModuleActionProvider : IActionProvider<ModuleAction, ModuleState>;
