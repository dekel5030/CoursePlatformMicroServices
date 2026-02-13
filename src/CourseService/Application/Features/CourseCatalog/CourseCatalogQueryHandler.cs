using Courses.Application.Abstractions.Data;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.ReadModels;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CourseCatalog;

internal sealed class CourseCatalogQueryHandler : IQueryHandler<CourseCatalogQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly ICourseSummaryDtoMapper _courseSummaryDtoMapper;

    public CourseCatalogQueryHandler(
        IReadDbContext dbContext,
        ICourseSummaryDtoMapper courseSummaryDtoMapper)
    {
        _dbContext = dbContext;
        _courseSummaryDtoMapper = courseSummaryDtoMapper;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        CourseCatalogQuery request,
        CancellationToken cancellationToken = default)
    {
        (int pageNumber, int pageSize) = PaginationOptions.Normalize(
            request.PagedQuery.Page,
            request.PagedQuery.PageSize);

        var query = _dbContext.Courses
            .OrderByDescending(course => course.UpdatedAtUtc)
            .Select(course => new
            {
                Course = course,
                Instructor = _dbContext.Users
                    .Where(user => user.Id == course.InstructorId)
                    .FirstOrDefault(),

                Category = _dbContext.Categories
                    .Where(category => category.Id == course.CategoryId)
                    .FirstOrDefault(),
            });

        int totalItems = await query.CountAsync(cancellationToken);

        var rawData = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var courseIds = rawData.Select(rawData => rawData.Course.Id.Value).ToList();

        Dictionary<Guid, CourseAnalytics> analyticsDict = await _dbContext.CourseAnalytics
            .Where(analytics => courseIds.Contains(analytics.CourseId))
            .ToDictionaryAsync(analytics => analytics.CourseId, cancellationToken);

        var courseDtos = rawData.Select(raw =>
        {
            analyticsDict.TryGetValue(raw.Course.Id.Value, out CourseAnalytics? stats);

            CourseSummaryDto summary = _courseSummaryDtoMapper.MapToCatalogSummary(
                raw.Course, raw.Instructor, raw.Category);
            CourseSummaryAnalyticsDto analytics = CourseAnalyticsDtoMapper.ToSummaryAnalytics(stats);

            return new CourseSummaryWithAnalyticsDto(summary, analytics);

        }).ToList();

        return Result.Success(new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            Links = []
        });
    }
}
