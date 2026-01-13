using System;
using System.Collections.Generic;
using System.Text;

namespace CoursePlatform.ServiceDefaults.Dtos;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}

public sealed record PagnitatedResponse<T> : ICollectionResponse<T>
{
    public required List<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
