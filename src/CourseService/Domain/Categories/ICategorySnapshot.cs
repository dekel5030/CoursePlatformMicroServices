using Courses.Domain.Categories.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Domain.Categories;

public interface ICategorySnapshot
{
    CategoryId Id { get; }
    string Name { get; }
    Slug Slug { get; }
}
