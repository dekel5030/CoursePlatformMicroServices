using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Features.Shared;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;
using Courses.Application.Users;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedCourses;

internal sealed class GetManagedCoursesQueryHandler
    : IQueryHandler<GetManagedCoursesQuery, PaginatedCollectionDto<ManagedCourseSummaryDto>>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly IUserContext _userContext;
    private readonly ILinkBuilderService _linkBuilder;

    public GetManagedCoursesQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver,
        IUserContext userContext,
        ILinkBuilderService linkBuilder)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
        _userContext = userContext;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<PaginatedCollectionDto<ManagedCourseSummaryDto>>> Handle(
        GetManagedCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        if (_userContext.Id is null || !_userContext.IsAuthenticated)
        {
            return Result.Failure<PaginatedCollectionDto<ManagedCourseSummaryDto>>(CourseErrors.Unauthorized);
        }

        var instructorId = new UserId(_userContext.Id.Value);
        (int pageNumber, int pageSize) = PaginationOptions.Normalize(request.PageNumber, request.PageSize);

        int totalItems = await _dbContext.Courses
            .Where(c => c.InstructorId == instructorId)
            .CountAsync(cancellationToken);

        List<ManagedCourseSummaryDto> courseDtos = await GetManagedCourseSummariesAsync(
            instructorId,
            pageNumber,
            pageSize,
            cancellationToken);

        return Result.Success(new PaginatedCollectionDto<ManagedCourseSummaryDto>
        {
            Items = courseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            Links = []
        });
    }

    private async Task<List<ManagedCourseSummaryDto>> GetManagedCourseSummariesAsync(
        UserId instructorId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        List<Course> courses = await _dbContext.Courses
            .Where(c => c.InstructorId == instructorId)
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (courses.Count == 0)
        {
            return [];
        }

        var courseIds = courses.Select(c => c.Id).ToList();
        var categoryIds = courses.Select(c => c.CategoryId).Distinct().ToList();

        Dictionary<UserId, User> instructors = await _dbContext.Users
            .Where(i => i.Id == instructorId)
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        Dictionary<CategoryId, Category> categories = await _dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var courseStats = await _dbContext.Lessons
            .Where(l => courseIds.Contains(l.CourseId))
            .GroupBy(l => l.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                LessonCount = g.Count(),
                TotalDurationSeconds = g.Sum(l => l.Duration.TotalSeconds)
            })
            .ToDictionaryAsync(x => x.CourseId, cancellationToken);

        var courseDtos = courses.Select(course =>
        {
            instructors.TryGetValue(course.InstructorId, out User? instructor);
            categories.TryGetValue(course.CategoryId, out Category? category);
            courseStats.TryGetValue(course.Id, out var stats);

            string? thumbnailUrl = CourseSummaryHelpers.GetFirstImagePublicUrl(course.Images, _urlResolver);
            string shortDescription = CourseSummaryHelpers.TruncateShortDescription(course.Description.Value);

            var courseContext = new CourseContext(course.Id, course.InstructorId, course.Status, IsManagementView: true);
            var links = _linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext).ToList();

            return new ManagedCourseSummaryDto
            {
                Id = course.Id.Value,
                Title = course.Title.Value,
                ShortDescription = shortDescription,
                Slug = course.Slug.Value,
                ThumbnailUrl = thumbnailUrl,
                Instructor = UserDtoMapper.Map(instructor, course.InstructorId.Value),
                Category = CategoryDtoMapper.Map(category),
                Difficulty = course.Difficulty,
                Price = course.Price,
                UpdatedAtUtc = course.UpdatedAtUtc,
                Status = course.Status,
                Stats = new ManagedCourseStatsDto(
                    LessonsCount: stats?.LessonCount ?? 0,
                    Duration: stats != null ? TimeSpan.FromSeconds(stats.TotalDurationSeconds) : TimeSpan.Zero),
                Links = links
            };
        }).ToList();

        return courseDtos;
    }
}
