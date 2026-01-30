using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface IModuleRepository : IRepository<Module, ModuleId>
{
    Task<IReadOnlyList<Module>> GetAllByCourseIdAsync(CourseId courseId, CancellationToken cancellationToken = default);
}
