using Courses.Application.Abstractions.Data;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.ReadModels;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Users;
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
        (int pageNumber, int pageSize) = PaginationOptions
            .Normalize(request.PagedQuery.Page, request.PagedQuery.PageSize);

        IQueryable<CatalogRawData> query = BuildCatalogQuery();

        int totalItems = await query.CountAsync(cancellationToken);
        List<CatalogRawData> rawData = await FetchPagedDataAsync(query, pageNumber, pageSize, cancellationToken);

        IEnumerable<Guid> courseIds = rawData.Select(data => data.Course.Id.Value);
        Dictionary<Guid, CourseAnalytics> analyticsDict = await GetAnalyticsLookupAsync(courseIds, cancellationToken);

        List<CourseSummaryWithAnalyticsDto> courseDtos = MapToCourseDtos(rawData, analyticsDict);

        return Result.Success(CreateCollectionDto(courseDtos, pageNumber, pageSize, totalItems));
    }

    private IQueryable<CatalogRawData> BuildCatalogQuery()
    {
        return _dbContext.Courses
            .OrderByDescending(course => course.UpdatedAtUtc)
            .Select(course => new CatalogRawData
            {
                Course = course,
                Instructor = _dbContext.Users.FirstOrDefault(user => user.Id == course.InstructorId),
                Category = _dbContext.Categories.FirstOrDefault(category => category.Id == course.CategoryId)
            });
    }

    private static async Task<List<CatalogRawData>> FetchPagedDataAsync(
        IQueryable<CatalogRawData> query,
        int page,
        int size,
        CancellationToken cancellationToken)
    {
        return await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    private async Task<Dictionary<Guid, CourseAnalytics>> GetAnalyticsLookupAsync(
        IEnumerable<Guid> courseIds,
        CancellationToken cancellationToken)
    {
        return await _dbContext.CourseAnalytics
            .Where(analytics => courseIds.Contains(analytics.CourseId))
            .ToDictionaryAsync(analytics => analytics.CourseId, cancellationToken);
    }

    private List<CourseSummaryWithAnalyticsDto> MapToCourseDtos(
        List<CatalogRawData> rawData, Dictionary<Guid, CourseAnalytics> analyticsDict)
    {
        return rawData.Select(raw =>
        {
            analyticsDict.TryGetValue(raw.Course.Id.Value, out CourseAnalytics? stats);

            CourseSummaryDto summary = _courseSummaryDtoMapper
                .MapToCatalogSummary(raw.Course, raw.Instructor, raw.Category);

            CourseSummaryAnalyticsDto analytics = CourseAnalyticsDtoMapper.ToSummaryAnalytics(stats);

            return new CourseSummaryWithAnalyticsDto(summary, analytics);
        }).ToList();
    }

    private static CourseCollectionDto CreateCollectionDto(
        List<CourseSummaryWithAnalyticsDto> items,
        int page,
        int size,
        int total)
    {
        return new CourseCollectionDto
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalItems = total,
            Links = []
        };
    }

    private sealed class CatalogRawData
    {
        public required Course Course { get; init; }
        public User? Instructor { get; init; }
        public Category? Category { get; init; }
    }
}
