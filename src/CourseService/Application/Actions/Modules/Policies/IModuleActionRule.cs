using Courses.Application.Actions.Abstract;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Modules.Policies;

public interface IModuleActionRule : IActionRule<ModuleAction, ModuleState>;
