using Courses.Domain.Shared;

namespace Courses.Application.Abstractions.Data.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : IEquatable<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}