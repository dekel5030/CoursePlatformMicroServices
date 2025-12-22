using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Queries.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, PagedResponseDto<CourseReadDto>>
{
    private readonly IReadDbContext _dbContext;

    public GetCoursesQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResponseDto<CourseReadDto>>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(1, request.PagedQuery.PageNumber ?? 1);
        var pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 100);

        var baseQuery = _dbContext.Courses;

        var totalItems = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return Result.Success(dto);
    }
}
