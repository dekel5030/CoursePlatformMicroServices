using Courses.Domain.Categories;
using Courses.Domain.Courses;

namespace Courses.Application.Abstractions.Repositories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task AddAsync(Category entity, CancellationToken cancellationToken = default);
}
