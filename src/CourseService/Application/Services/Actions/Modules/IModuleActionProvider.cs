using Courses.Application.Services.Actions.Abstractions;

namespace Courses.Application.Services.Actions.Modules;

public interface IModuleActionProvider : IActionProvider<ModuleAction, ModuleState>;
