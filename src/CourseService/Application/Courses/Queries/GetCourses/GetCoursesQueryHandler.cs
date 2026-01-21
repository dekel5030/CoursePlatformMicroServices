using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCoursesQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.PageNumber ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 100);

        DbSet<Course> baseQuery = _dbContext.Courses;

        int totalItems = await baseQuery.CountAsync(cancellationToken);

        List<CourseSummaryDto> courses = await _dbContext.Courses
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(ProjectionMappings.ToCourseSummary)
            .ToListAsync(cancellationToken);

        courses = courses.Select(course => course.EnrichWithUrls(_urlResolver)).ToList();

        var response = new CourseCollectionDto(
            courses,
            pageNumber,
            pageSize,
            totalItems);

        return Result.Success(response);
    }
}
