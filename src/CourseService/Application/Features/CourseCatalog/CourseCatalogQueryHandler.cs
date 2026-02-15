using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Features.CourseCatalog;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Users;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CourseCatalog;

internal sealed class CourseCatalogQueryHandler : IQueryHandler<CourseCatalogQuery, CourseCatalogDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly ILinkProvider _linkProvider;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public CourseCatalogQueryHandler(
        IReadDbContext dbContext,
        ILinkProvider linkProvider,
        IStorageUrlResolver storageUrlResolver)
    {
        _dbContext = dbContext;
        _linkProvider = linkProvider;
        _storageUrlResolver = storageUrlResolver;
    }

    public async Task<Result<CourseCatalogDto>> Handle(
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

        List<CourseCatalogItemDto> items = MapToItemDtos(rawData, analyticsDict);

        bool hasNext = pageNumber * pageSize < totalItems;
        bool hasPrev = pageNumber > 1;
        var collectionLinks = new CourseCatalogCollectionLinks(
            Self: _linkProvider.GetCoursesLink(pageNumber, pageSize),
            Next: hasNext ? _linkProvider.GetCoursesLink(pageNumber + 1, pageSize) : null,
            Prev: hasPrev ? _linkProvider.GetCoursesLink(pageNumber - 1, pageSize) : null);

        var dto = new CourseCatalogDto(
            Items: items,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalItems: totalItems,
            Links: collectionLinks);

        return Result.Success(dto);
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

    private List<CourseCatalogItemDto> MapToItemDtos(
        List<CatalogRawData> rawData,
        Dictionary<Guid, CourseAnalytics> analyticsDict)
    {
        return rawData.Select(raw =>
        {
            analyticsDict.TryGetValue(raw.Course.Id.Value, out CourseAnalytics? stats);
            CourseCatalogItemData data = MapItemData(raw, stats);
            Guid courseId = raw.Course.Id.Value;
            var links = new CourseCatalogItemLinks(
                Self: _linkProvider.GetCoursePageLink(courseId),
                Watch: _linkProvider.GetCoursePageLink(courseId));
            return new CourseCatalogItemDto(Data: data, Links: links);
        }).ToList();
    }

    private CourseCatalogItemData MapItemData(CatalogRawData raw, CourseAnalytics? stats)
    {
        string? thumbnailUrl = CourseSummaryHelpers.GetFirstImagePublicUrl(raw.Course.Images, _storageUrlResolver);
        string shortDescription = CourseSummaryHelpers.TruncateShortDescription(raw.Course.Description.Value);

        return new CourseCatalogItemData(
            Id: raw.Course.Id.Value,
            Title: raw.Course.Title.Value,
            ShortDescription: shortDescription,
            Slug: raw.Course.Slug.Value,
            Instructor: UserDtoMapper.Map(raw.Instructor, raw.Course.InstructorId.Value),
            Category: CategoryDtoMapper.Map(raw.Category),
            Price: raw.Course.Price,
            Difficulty: raw.Course.Difficulty,
            ThumbnailUrl: thumbnailUrl,
            UpdatedAtUtc: raw.Course.UpdatedAtUtc,
            Status: raw.Course.Status,
            LessonsCount: stats?.TotalLessonsCount ?? 0,
            Duration: stats?.TotalCourseDuration ?? TimeSpan.Zero,
            EnrollmentCount: stats?.EnrollmentsCount ?? 0,
            AverageRating: stats?.AverageRating ?? 0,
            ReviewsCount: stats?.ReviewsCount ?? 0,
            CourseViews: stats?.ViewCount ?? 0);
    }

    private sealed class CatalogRawData
    {
        public required Course Course { get; init; }
        public User? Instructor { get; init; }
        public Category? Category { get; init; }
    }
}
