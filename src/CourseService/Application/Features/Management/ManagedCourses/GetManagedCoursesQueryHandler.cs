using Courses.Application.Abstractions.Data;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.ReadModels;
using Courses.Application.Shared.Dtos;
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
    private readonly ICourseSummaryDtoMapper _courseSummaryDtoMapper;
    private readonly IUserContext _userContext;

    public GetManagedCoursesQueryHandler(
        IReadDbContext dbContext,
        ICourseSummaryDtoMapper courseSummaryDtoMapper,
        IUserContext userContext)
    {
        _dbContext = dbContext;
        _courseSummaryDtoMapper = courseSummaryDtoMapper;
        _userContext = userContext;
    }
    public async Task<Result<PaginatedCollectionDto<ManagedCourseSummaryDto>>> Handle(
        GetManagedCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        if (!_userContext.IsAuthenticated || _userContext.Id is null)
        {
            return Result.Failure<PaginatedCollectionDto<ManagedCourseSummaryDto>>(CourseErrors.Unauthorized);
        }

        var instructorId = new UserId(_userContext.Id.Value);
        (int pageNumber, int pageSize) = PaginationOptions.Normalize(request.PageNumber, request.PageSize);

        int totalItems = await GetTotalCoursesCountAsync(instructorId, cancellationToken);
        if (totalItems == 0)
        {
            return Result.Success(CreateEmptyResponse(pageNumber, pageSize));
        }

        List<Course> courses =
            await FetchPagedCoursesAsync(instructorId, pageNumber, pageSize, cancellationToken);

        Dictionary<CategoryId, Category> categories = await GetCategoryMapAsync(courses, cancellationToken);
        Dictionary<Guid, CourseAnalytics> analytics = await GetAnalyticsMapAsync(courses, cancellationToken);
        User? instructor = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == instructorId, cancellationToken);

        List<ManagedCourseSummaryDto> dtos =
            MapToManagedSummaryDtos(courses, instructor, categories, analytics);

        return Result.Success(new PaginatedCollectionDto<ManagedCourseSummaryDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            Links = []
        });
    }

    private async Task<int> GetTotalCoursesCountAsync(
        UserId instructorId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .CountAsync(course => course.InstructorId == instructorId, cancellationToken);
    }

    private async Task<List<Course>> FetchPagedCoursesAsync(
        UserId instructorId,
        int page,
        int size,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .Where(course => course.InstructorId == instructorId)
            .OrderByDescending(course => course.UpdatedAtUtc)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    private async Task<Dictionary<CategoryId, Category>> GetCategoryMapAsync(
        List<Course> courses,
        CancellationToken cancellationToken = default)
    {
        var categoryIds = courses.Select(course => course.CategoryId).Distinct().ToList();
        return await _dbContext.Categories
            .Where(course => categoryIds.Contains(course.Id))
            .ToDictionaryAsync(course => course.Id, cancellationToken);
    }

    private async Task<Dictionary<Guid, CourseAnalytics>> GetAnalyticsMapAsync(
        List<Course> courses,
        CancellationToken cancellationToken = default)
    {
        var courseIds = courses.Select(c => c.Id.Value).ToList();
        return await _dbContext.CourseAnalytics
            .Where(analytics => courseIds.Contains(analytics.CourseId))
            .ToDictionaryAsync(analytics => analytics.CourseId, cancellationToken);
    }

    private List<ManagedCourseSummaryDto> MapToManagedSummaryDtos(
        List<Course> courses,
        User? instructor,
        Dictionary<CategoryId, Category> categories,
        Dictionary<Guid, CourseAnalytics> analytics)
    {
        return courses.Select(course =>
        {
            categories.TryGetValue(course.CategoryId, out Category? category);
            analytics.TryGetValue(course.Id.Value, out CourseAnalytics? stats);

            var statsDto = new ManagedCourseStatsDto(
                LessonsCount: stats?.TotalLessonsCount ?? 0,
                Duration: stats?.TotalCourseDuration ?? TimeSpan.Zero);

            return _courseSummaryDtoMapper.MapToManagedSummary(course, instructor, category, statsDto);
        }).ToList();
    }

    private static PaginatedCollectionDto<ManagedCourseSummaryDto> CreateEmptyResponse(
        int page,
        int size)
    {
        return new()
        {
            Items = [],
            PageNumber = page,
            PageSize = size,
            TotalItems = 0,
            Links = []
        };
    }
}
