using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface IModuleRepository : IRepository<Module, ModuleId>
{
    Task AddAsync(Module entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Module>> GetAllByCourseIdAsync(CourseId courseId, CancellationToken cancellationToken = default);
}
