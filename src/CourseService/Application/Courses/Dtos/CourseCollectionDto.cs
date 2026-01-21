using Courses.Application.Shared.Dtos;

namespace Courses.Application.Courses.Dtos;

public sealed record CourseCollectionDto(
    IReadOnlyCollection<CourseSummaryDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems
) : CollectionDto<CourseSummaryDto>(
    Items,
    PageNumber,
    PageSize,
    TotalItems
);
