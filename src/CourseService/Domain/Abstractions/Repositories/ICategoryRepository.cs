using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface ICategoryRepository : IRepository<Category, CategoryId>;
