using Courses.Domain.Categories;

namespace Courses.Application.Abstractions.Repositories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task AddAsync(Category entity, CancellationToken cancellationToken = default);
}
