using System.Linq.Expressions;
using Courses.Domain.Shared;

namespace Courses.Domain.Abstractions.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : IHasId<TId>
    where TId : IEquatable<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    void Remove(TEntity entity);
}
