using Courses.Domain.Courses.Primitives;
using Courses.Domain.Module.Primitives;

namespace Courses.Application.Services.Actions.Modules;

public sealed record ModuleState(CourseId CourseId, ModuleId ModuleId);
