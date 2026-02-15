using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories;
using Courses.Application.Features.Management;
using Courses.Application.Features.Management.ManagedCourses;
using Courses.Application.Features.Shared;
using Courses.Application.ReadModels;
using Courses.Application.Services.LinkProvider.Abstractions;
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
    : IQueryHandler<GetManagedCoursesQuery, GetManagedCoursesDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly ILinkProvider _linkProvider;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public GetManagedCoursesQueryHandler(
        IReadDbContext dbContext,
        IUserContext userContext,
        ILinkProvider linkProvider,
        IStorageUrlResolver storageUrlResolver)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _linkProvider = linkProvider;
        _storageUrlResolver = storageUrlResolver;
    }

    public async Task<Result<GetManagedCoursesDto>> Handle(
        GetManagedCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        if (!_userContext.IsAuthenticated || _userContext.Id is null)
        {
            return Result.Failure<GetManagedCoursesDto>(CourseErrors.Unauthorized);
        }

        var instructorId = new UserId(_userContext.Id.Value);
        (int pageNumber, int pageSize) = PaginationOptions.Normalize(request.PageNumber, request.PageSize);

        int totalItems = await GetTotalCoursesCountAsync(instructorId, cancellationToken);
        if (totalItems == 0)
        {
            var emptyLinks = new GetManagedCoursesCollectionLinks(
                Self: _linkProvider.GetManagedCoursesLink(pageNumber, pageSize));
            return Result.Success(new GetManagedCoursesDto(
                Items: [],
                PageNumber: pageNumber,
                PageSize: pageSize,
                TotalItems: 0,
                Links: emptyLinks));
        }

        List<Course> courses =
            await FetchPagedCoursesAsync(instructorId, pageNumber, pageSize, cancellationToken);

        Dictionary<CategoryId, Category> categories = await GetCategoryMapAsync(courses, cancellationToken);
        Dictionary<Guid, CourseAnalytics> analytics = await GetAnalyticsMapAsync(courses, cancellationToken);
        User? instructor = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == instructorId, cancellationToken);

        List<ManagedCourseSummaryItemDto> items =
            MapToItemDtos(courses, instructor, categories, analytics);

        var collectionLinks = new GetManagedCoursesCollectionLinks(
            Self: _linkProvider.GetManagedCoursesLink(pageNumber, pageSize));

        return Result.Success(new GetManagedCoursesDto(
            Items: items,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalItems: totalItems,
            Links: collectionLinks));
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
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);
    }

    private async Task<Dictionary<Guid, CourseAnalytics>> GetAnalyticsMapAsync(
        List<Course> courses,
        CancellationToken cancellationToken = default)
    {
        var courseIds = courses.Select(c => c.Id.Value).ToList();
        return await _dbContext.CourseAnalytics
            .Where(a => courseIds.Contains(a.CourseId))
            .ToDictionaryAsync(a => a.CourseId, cancellationToken);
    }

    private List<ManagedCourseSummaryItemDto> MapToItemDtos(
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

            ManagedCourseSummaryData data = MapSummaryData(course, instructor, category, statsDto);
            var links = new ManagedCourseSummaryLinks(
                Self: _linkProvider.GetManagedCourseLink(course.Id.Value),
                CoursePage: _linkProvider.GetCoursePageLink(course.Id.Value));

            return new ManagedCourseSummaryItemDto(Data: data, Links: links);
        }).ToList();
    }

    private ManagedCourseSummaryData MapSummaryData(
        Course course,
        User? instructor,
        Category? category,
        ManagedCourseStatsDto stats)
    {
        string? thumbnailUrl = CourseSummaryHelpers.GetFirstImagePublicUrl(course.Images, _storageUrlResolver);
        string shortDescription = CourseSummaryHelpers.TruncateShortDescription(course.Description.Value);

        return new ManagedCourseSummaryData(
            Id: course.Id.Value,
            Title: course.Title.Value,
            ShortDescription: shortDescription,
            Slug: course.Slug.Value,
            Instructor: UserDtoMapper.Map(instructor, course.InstructorId.Value),
            Category: CategoryDtoMapper.Map(category),
            Price: course.Price,
            Difficulty: course.Difficulty,
            ThumbnailUrl: thumbnailUrl,
            UpdatedAtUtc: course.UpdatedAtUtc,
            Status: course.Status,
            Stats: stats);
    }
}