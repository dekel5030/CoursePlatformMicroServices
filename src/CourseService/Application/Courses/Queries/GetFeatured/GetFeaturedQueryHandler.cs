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

    // Keep featured IDs as CourseId so EF can apply the value converter on c.Id.
    private static readonly List<CourseId> _featuredCourseIds = new()
    {
        // new CourseId(Guid.Parse("11111111-1111-1111-1111-111111111111")),
        // new CourseId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
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
                c.UpdatedAtUtc))
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