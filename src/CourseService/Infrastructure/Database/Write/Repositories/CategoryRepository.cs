using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;

namespace Courses.Infrastructure.Database.Write.Repositories;

internal sealed class CategoryRepository : RepositoryBase<Category, CategoryId>, ICategoryRepository
{
    public CategoryRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
