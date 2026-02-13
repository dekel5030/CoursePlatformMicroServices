using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Features.Dtos;
using Courses.Application.Features.Shared;
using Courses.Application.ReadModels;
using Courses.Application.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CourseCatalog;

internal sealed class CourseCatalogQueryHandler : IQueryHandler<CourseCatalogQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public CourseCatalogQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
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

            string? thumbnail = CourseSummaryHelpers.GetFirstImagePublicUrl(raw.Course.Images, _urlResolver);
            string shortDesc = CourseSummaryHelpers.TruncateShortDescription(raw.Course.Description.Value);

            var summary = new CourseSummaryDto
            {
                Id = raw.Course.Id.Value,
                Title = raw.Course.Title.Value,
                ShortDescription = shortDesc,
                Slug = raw.Course.Slug.Value,
                ThumbnailUrl = thumbnail,
                Instructor = UserDtoMapper.Map(raw.Instructor),
                Category = CategoryDtoMapper.Map(raw.Category),
                Difficulty = raw.Course.Difficulty,
                Price = raw.Course.Price,
                UpdatedAtUtc = raw.Course.UpdatedAtUtc,
                Status = raw.Course.Status,
                Links = []
            };

            CourseSummaryAnalyticsDto analytics = MapAnalytics(stats);

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

    private static CourseSummaryAnalyticsDto MapAnalytics(CourseAnalytics? stats)
    {
        return new CourseSummaryAnalyticsDto(
            LessonsCount: stats?.TotalLessonsCount ?? 0,
            Duration: stats?.TotalCourseDuration ?? TimeSpan.Zero,
            EnrollmentCount: stats?.EnrollmentsCount ?? 0,
            AverageRating: stats?.AverageRating ?? 0,
            ReviewsCount: stats?.ReviewsCount ?? 0,
            CourseViews: stats?.ViewCount ?? 0);
    }
}
