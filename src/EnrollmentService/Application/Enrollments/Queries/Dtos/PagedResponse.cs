namespace Application.Enrollments.Queries.Dtos;

public record PagedResponse<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
}
