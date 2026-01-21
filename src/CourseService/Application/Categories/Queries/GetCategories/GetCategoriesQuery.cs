using Courses.Application.Categories.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery() : IQuery<IReadOnlyList<CategoryDto>>;
