using Courses.Application.Categories.Dtos;
using Courses.Domain.Categories;

namespace Courses.Application.Categories;

public static class CategoryDtoMapper
{
    public static CategoryDto Map(Category? category)
    {
        if (category is null)
        {
            return new CategoryDto(Guid.Empty, "Unknown", "unknown");
        }

        return new CategoryDto(category.Id.Value, category.Name, category.Slug.Value);
    }
}
