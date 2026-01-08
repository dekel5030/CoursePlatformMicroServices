using Kernel;

namespace Courses.Infrastructure.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : Entity
{
    public Task<TEntity?> GetByidAsync(TId id, CancellationToken cancellationToken = default);
    public void Add(TEntity entity);
}
