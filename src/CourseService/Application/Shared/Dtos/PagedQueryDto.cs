namespace Courses.Application.Shared.Dtos;

public record PagedQueryDto
{
    public int? PageNumber { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
}