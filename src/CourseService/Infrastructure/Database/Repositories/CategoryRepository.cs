using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Categories;

namespace Courses.Infrastructure.Database.Repositories;

internal sealed class CategoryRepository : RepositoryBase<Category, CategoryId>, ICategoryRepository
{
    public CategoryRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
