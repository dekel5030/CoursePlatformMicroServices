using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Contracts.Shared;

internal sealed record PagedResponse<T> : ILinksResponse
{
    public List<T> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;

    public required IReadOnlyCollection<LinkDto> Links { get; init; }
}
