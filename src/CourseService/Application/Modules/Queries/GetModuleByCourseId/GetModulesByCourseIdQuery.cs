using Courses.Application.Courses.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Queries.GetByCourseId;

public sealed record GetModulesByCourseIdQuery(CourseId CourseId) : IQuery<IReadOnlyList<ModuleDto>>;
