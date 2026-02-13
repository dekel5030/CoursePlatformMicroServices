namespace Courses.Application.Features.Shared;

public static class PaginationOptions
{
    public const int DefaultMaxPageSize = 25;
    public const int DefaultPageSize = 10;

    public static (int PageNumber, int PageSize) Normalize(
        int? page,
        int? pageSize,
        int maxPageSize = DefaultMaxPageSize)
    {
        int pageNumber = Math.Max(1, page ?? 1);
        int size = Math.Clamp(pageSize ?? DefaultPageSize, 1, maxPageSize);
        return (pageNumber, size);
    }
}
