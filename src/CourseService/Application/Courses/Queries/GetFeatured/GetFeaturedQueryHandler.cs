using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Domain.Courses.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Courses.Queries.GetFeatured;

public class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>>
{
    private readonly IReadDbContext _dbContext;

    private static readonly List<CourseId> _featuredCourseIds = new()
    {
        new CourseId(Guid.Parse("019a6842-b9a5-714a-b257-d164982cbc19")),
        new CourseId(Guid.Parse("019a6842-95e4-76bd-ac68-7884af14ae5f")),
        new CourseId(Guid.Parse("019a778e-4e98-7e5a-8e86-c2fc7b756f70")),
        new CourseId(Guid.Parse("019a778e-ea05-7b08-9308-f381eae6b750")),
    };

    public GetFeaturedQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResponseDto<CourseReadDto>>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        if (_featuredCourseIds.Count == 0)
        {
            return Result.Success(new PagedResponseDto<CourseReadDto>
            {
                Items = new(),
                PageNumber = 1,
                PageSize = 1,
                TotalItems = 0
            });
        }

        var featuredIds = _featuredCourseIds.ToArray();

        var items = await _dbContext.Courses
            .Where(c => featuredIds.Contains(c.Id)) 
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Select(c => new CourseReadDto(
                c.Id,
                c.Title,
                c.Description,
                c.IsPublished,
                c.ImageUrl,
                c.InstructorUserId,
                c.Price,
                c.UpdatedAtUtc,
                null))
            .ToListAsync(cancellationToken);

        var dto = new PagedResponseDto<CourseReadDto>
        {
            Items = items,
            PageNumber = 1,
            PageSize = Math.Max(1, items.Count),
            TotalItems = items.Count
        };

        return Result.Success(dto);
    }
}