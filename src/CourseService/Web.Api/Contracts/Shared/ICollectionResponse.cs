using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Contracts.Shared;

internal interface ICollectionResponse<T>
{
    IReadOnlyCollection<T> Items { get; init; }
    int PageNumber { get; init; }
    int PageSize { get; init; }
    int TotalItems { get; init; }
    LinkDto? Next { get; init; }
    LinkDto? Previous { get; init; }
}
