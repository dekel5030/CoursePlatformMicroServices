using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourseSummaries;

internal sealed class GetCourseSummariesQueryHandler : IQueryHandler<GetCourseSummariesQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseSummariesQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCourseSummariesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.Page ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 25);

        int totalItems = await _dbContext.Courses.CountAsync(cancellationToken);

        List<CourseSummaryDto> courseDtos = await GetCourseSummariesAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Result.Success(new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            Links = []
        });
    }

    private async Task<List<CourseSummaryDto>> GetCourseSummariesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        List<Course> courses = await _dbContext.Courses
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (courses.Count == 0)
        {
            return [];
        }

        var courseIds = courses.Select(c => c.Id).ToList();
        var instructorIds = courses.Select(c => c.InstructorId).Distinct().ToList();
        var categoryIds = courses.Select(c => c.CategoryId).Distinct().ToList();

        Dictionary<UserId, User> instructors = await _dbContext.Users
            .Where(i => instructorIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        Dictionary<CategoryId, Category> categories = await _dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var courseStats = await _dbContext.Modules
            .Where(m => courseIds.Contains(m.CourseId))
            .GroupBy(m => m.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                LessonCount = g.Sum(m => m.Lessons.Count),
                TotalDurationSeconds = g.Sum(m => m.Lessons.Sum(l => l.Duration.TotalSeconds))
            })
            .ToDictionaryAsync(x => x.CourseId, cancellationToken);

        var enrollmentCounts = await _dbContext.Enrollments
            .Where(e => courseIds.Contains(e.CourseId))
            .GroupBy(e => e.CourseId)
            .Select(g => new { CourseId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CourseId, cancellationToken);

        var courseDtos = courses.Select(course =>
        {
            instructors.TryGetValue(course.InstructorId, out User? instructor);
            categories.TryGetValue(course.CategoryId, out Category? category);
            courseStats.TryGetValue(course.Id, out var stats);
            enrollmentCounts.TryGetValue(course.Id, out var enrollmentCount);

            string? thumbnailUrl = course.Images.Count > 0
                ? _urlResolver.Resolve(StorageCategory.Public, course.Images.First().Path).Value
                : null;

            return new CourseSummaryDto
            {
                Id = course.Id.Value,
                Title = course.Title.Value,
                ShortDescription = course.Description.Value.Length > 150
                    ? course.Description.Value[..150] + "..."
                    : course.Description.Value,
                Slug = course.Slug.Value,
                ThumbnailUrl = thumbnailUrl,
                Instructor = new InstructorDto(
                    instructor?.Id.Value ?? Guid.Empty,
                    instructor?.FullName ?? "Unknown",
                    instructor?.AvatarUrl),
                Category = new CategoryDto(
                    category?.Id.Value ?? Guid.Empty,
                    category?.Name ?? "Uncategorized",
                    category?.Slug.Value ?? string.Empty),
                Difficulty = course.Difficulty,
                Price = course.Price,
                OriginalPrice = null,
                Badges = [],
                AverageRating = 4.5,
                ReviewsCount = 2157,
                LessonsCount = stats?.LessonCount ?? 0,
                Duration = stats != null ? TimeSpan.FromSeconds(stats.TotalDurationSeconds) : TimeSpan.Zero,
                EnrollmentCount = enrollmentCount?.Count ?? 0,
                CourseViews = 0,
                UpdatedAtUtc = course.UpdatedAtUtc,
                Status = course.Status,
                Links = []
            };
        }).ToList();

        return courseDtos;
    }
}
