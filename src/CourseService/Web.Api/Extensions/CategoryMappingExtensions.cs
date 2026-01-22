using Courses.Api.Contracts.Categories;
using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Categories.Dtos;

namespace Courses.Api.Extensions;

internal static class CategoryMappingExtensions
{
    public static CategoryResponse ToApiContract(
        this CategoryDto dto,
        LinkProvider linkProvider)
    {
        return new CategoryResponse(
            dto.Id.Value,
            dto.Name,
            dto.Slug.Value,
            linkProvider.CreateCategoryLinks(dto.Id));
    }

    public static PagedResponse<CategoryResponse> ToApiContract(
        this CategoryCollectionDto dto,
        LinkProvider linkProvider)
    {
        return new PagedResponse<CategoryResponse>
        {
            Items = dto.Items.Select(item => item.ToApiContract(linkProvider)).ToList(),
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            TotalItems = dto.TotalItems,
            Links = linkProvider.CreateCategoryCollectionLinks()
        };
    }
}
