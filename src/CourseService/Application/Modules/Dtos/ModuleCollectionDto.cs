using Courses.Application.Shared.Dtos;

namespace Courses.Application.Modules.Dtos;

public sealed record ModuleCollectionDto(
    IReadOnlyCollection<ModuleDetailsDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems)
    : CollectionDto<ModuleDetailsDto>(Items, PageNumber, PageSize, TotalItems);
