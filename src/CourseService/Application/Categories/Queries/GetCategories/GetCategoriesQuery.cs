using Courses.Application.Categories.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetCategories;

public sealed record CategoryFilter(
    CourseId? CourseId = null,
    IEnumerable<Guid>? Ids = null);

public sealed record GetCategoriesQuery(CategoryFilter Filter) : IQuery<IReadOnlyList<CategoryDto>>;
