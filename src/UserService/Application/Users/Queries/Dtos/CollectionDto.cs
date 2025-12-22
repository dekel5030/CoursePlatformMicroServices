namespace Users.Application.Users.Queries.Dtos;

public record CollectionDto<T>
{
    public required IEnumerable<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 10;
}