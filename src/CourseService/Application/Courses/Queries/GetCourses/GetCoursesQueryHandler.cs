using Courses.Application.Abstractions;
using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Queries.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, PagedResponseDto<CourseSummaryDto>>
{
    private readonly IReadDbContext _dbContext;
    private readonly IUrlResolver _urlResolver;

    public GetCoursesQueryHandler(IReadDbContext dbContext, IUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<PagedResponseDto<CourseSummaryDto>>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(1, request.PagedQuery.PageNumber ?? 1);
        var pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 100);

        var baseQuery = _dbContext.Courses;

        int totalItems = await baseQuery.CountAsync(cancellationToken);

        var itemsData = await baseQuery
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Price,
                c.EnrollmentCount,
                c.InstructorId,
                Images = c.Images,
                LessonsCount = c.Lessons.Count
            })
            .ToListAsync(cancellationToken);

                var items = itemsData.Select(c => new CourseSummaryDto(
                    c.Id.Value,
                    c.Title.Value,
                    c.InstructorId?.Value.ToString(),
                    c.Price.Amount,
                    c.Price.Currency,
                    _urlResolver.Resolve(c.Images?.FirstOrDefault()?.Path ?? string.Empty),
                    c.LessonsCount,
                    c.EnrollmentCount
                )).ToList();

        var dto = new PagedResponseDto<CourseSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return Result.Success(dto);
    }
}