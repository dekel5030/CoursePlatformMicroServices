using Courses.Application.Modules.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Queries.GetModules;

public sealed record ModuleFilter(
    CourseId? CourseId = null,
    IEnumerable<Guid>? Ids = null);

public sealed record GetModulesQuery(ModuleFilter Filter) : IQuery<IReadOnlyList<ModuleDto>>;
