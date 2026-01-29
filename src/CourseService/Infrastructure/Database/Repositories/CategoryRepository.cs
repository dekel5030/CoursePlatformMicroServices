using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Courses.Infrastructure.Database.Write;

namespace Courses.Infrastructure.Database.Repositories;

internal sealed class CategoryRepository : RepositoryBase<Category, CategoryId>, ICategoryRepository
{
    public CategoryRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
