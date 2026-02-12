using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Features.Dtos;
using Courses.Application.ReadModels;
using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Primitives;
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
        int pageNumber = Math.Max(1, request.PagedQuery.Page ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 25);

        var query = _dbContext.Courses
            .OrderByDescending(course => course.UpdatedAtUtc)
            .Select(course => new
            {
                Course = course,
                Instructor = _dbContext.Users
                    .Where(user => user.Id == course.InstructorId)
                    .Select(user => new { user.Id.Value, user.FirstName, user.LastName, user.AvatarUrl })
                    .FirstOrDefault(),

                Category = _dbContext.Categories
                    .Where(category => category.Id == course.CategoryId)
                    .Select(category => new { category.Id.Value, category.Name, Slug = category.Slug.Value })
                    .FirstOrDefault(),
            });

        int totalItems = await query.CountAsync(cancellationToken);

        var rawData = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var courseIds = rawData.Select(r => r.Course.Id.Value).ToList();

        Dictionary<Guid, CourseAnalytics> analyticsDict = await _dbContext.CourseAnalytics
            .Where(a => courseIds.Contains(a.CourseId))
            .ToDictionaryAsync(a => a.CourseId, cancellationToken);

        var courseDtos = rawData.Select(raw =>
        {
            analyticsDict.TryGetValue(raw.Course.Id.Value, out CourseAnalytics? stats);

            string? thumbnail = raw.Course.Images != null && raw.Course.Images.Count > 0
                ? _urlResolver.Resolve(StorageCategory.Public, raw.Course.Images.FirstOrDefault()!.Path).Value
                : null;

            string shortDesc = raw.Course.Description.Value.Length > 100
                ? raw.Course.Description.Value[..100] + "..."
                : raw.Course.Description.Value;

            var summary = new CourseSummaryDto
            {
                Id = raw.Course.Id.Value,
                Title = raw.Course.Title.Value,
                ShortDescription = shortDesc,
                Slug = raw.Course.Slug.Value,
                ThumbnailUrl = thumbnail,
                Instructor = new UserDto
                {
                    Id = raw.Instructor?.Value ?? Guid.Empty,
                    FirstName = raw.Instructor?.FirstName ?? "Unknown",
                    LastName = raw.Instructor?.LastName ?? "Instructor",
                    AvatarUrl = raw.Instructor?.AvatarUrl
                },
                Category = new CategoryDto(
                    raw.Category?.Value ?? Guid.Empty,
                    raw.Category?.Name ?? "Uncategorized",
                    raw.Category?.Slug ?? string.Empty),
                Difficulty = raw.Course.Difficulty,
                Price = raw.Course.Price,
                UpdatedAtUtc = raw.Course.UpdatedAtUtc,
                Status = raw.Course.Status,
                Links = []
            };

            var analytics = new CourseSummaryAnalyticsDto(
                LessonsCount: stats?.TotalLessonsCount ?? 0,
                Duration: stats?.TotalCourseDuration ?? TimeSpan.Zero,
                EnrollmentCount: stats?.EnrollmentsCount ?? 0,
                AverageRating: stats?.AverageRating ?? 0,
                ReviewsCount: stats?.ReviewsCount ?? 0,
                CourseViews: stats?.ViewCount ?? 0);

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
