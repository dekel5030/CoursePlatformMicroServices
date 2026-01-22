using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Modules.Commands.CreateModule;

public sealed record CreateModuleResponse(ModuleId ModuleId, CourseId CourseId, Title Title);
