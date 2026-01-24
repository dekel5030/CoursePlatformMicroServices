using Courses.Application.Services.Actions.Abstractions;

namespace Courses.Application.Services.Actions.Modules.Policies;

public interface IModuleActionRule : IActionRule<ModuleAction, ModuleState>;
