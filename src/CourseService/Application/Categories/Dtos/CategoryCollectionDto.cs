using Courses.Application.Shared.Dtos;

namespace Courses.Application.Categories.Dtos;

public record CategoryCollectionDto(
    IReadOnlyCollection<CategoryDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems) : CollectionDto<CategoryDto>(Items, PageNumber, PageSize, TotalItems);
