using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.Repositories;
using Courses.Application.Courses.Queries.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetFeatured;

public class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>>
{
    private readonly IReadDbContext _dbContext;
    private readonly IFeaturedCoursesRepository _featuredCoursesProvider;

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