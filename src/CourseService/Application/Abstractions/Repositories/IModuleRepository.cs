using Courses.Domain.Module;
using Courses.Domain.Module.Primitives;

namespace Courses.Application.Abstractions.Repositories;

public interface IModuleRepository : IRepository<Module, ModuleId>
{
    Task AddAsync(Module entity, CancellationToken cancellationToken = default);
}
