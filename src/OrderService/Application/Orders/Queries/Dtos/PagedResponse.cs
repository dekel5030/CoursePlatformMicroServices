namespace Application.Orders.Queries.Dtos;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNext => PageNumber < TotalPages;
    public bool HasPrevious => PageNumber > 1;
}
