using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.PatchModule;

public record PatchModuleCommand(ModuleId ModuleId, Title? Title) : ICommand;
