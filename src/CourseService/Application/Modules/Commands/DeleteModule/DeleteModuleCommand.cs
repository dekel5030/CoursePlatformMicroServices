using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.DeleteModule;

public record DeleteModuleCommand(ModuleId ModuleId) : ICommand;
