using Courses.Domain.Categories;
using Courses.Domain.Shared.Primitives;

namespace Courses.Application.Categories.Dtos;

public record CategoryDto(
    CategoryId Id,
    string Name,
    Slug Slug
);
