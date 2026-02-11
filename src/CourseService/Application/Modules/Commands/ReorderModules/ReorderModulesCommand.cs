using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Commands.ReorderModules;

public sealed record ReorderModulesCommand(CourseId CourseId, List<ModuleId> ModuleIds) : ICommand;
