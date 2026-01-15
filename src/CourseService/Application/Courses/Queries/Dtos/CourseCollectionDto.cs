using Courses.Application.Shared.Dtos;

namespace Courses.Application.Courses.Queries.Dtos;

public sealed record CourseCollectionDto(
    IReadOnlyCollection<CourseSummaryDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    IReadOnlyList<CourseCollectionAction> AllowedActions
) : CollectionDto<CourseSummaryDto>(
    Items,
    PageNumber,
    PageSize,
    TotalItems
);