namespace Courses.Application.Shared.Dtos;

public record PagedQueryDto
{
    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
}
