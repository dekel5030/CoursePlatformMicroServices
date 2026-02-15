using Courses.Application.Categories.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery(CategoryFilter Filter) : IQuery<IReadOnlyList<CategoryDto>>;

public sealed record CategoryFilter(IEnumerable<Guid>? Ids = null);

