namespace EnrollmentService.Dtos;

public sealed record class PagedResponseDto<T>
{
    public required IEnumerable<T> Items { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
