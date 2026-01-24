using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Modules.Commands.CreateModule;

public sealed record CreateModuleResponse(Guid ModuleId, Guid CourseId, string Title);
