using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Shared.Dtos;

public record PaginatedCollectionDto<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalItems { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public required IReadOnlyList<LinkDto>? Links { get; init; }
}
