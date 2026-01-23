namespace Courses.Application.Shared.Dtos;

public record PaginatedCollectionDto<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    int TotalItems)
{
    public bool HasNextPage => TotalItems > PageNumber * PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
