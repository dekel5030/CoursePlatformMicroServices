using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Shared.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

/// <summary>
/// Query Handler for GetCourses - uses composition pattern with Core Read Models.
/// </summary>
internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ICourseLinkFactory _courseLinkFactory;

    public GetCoursesQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        ICourseLinkFactory courseLinkFactory)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _courseLinkFactory = courseLinkFactory;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.Page ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 25);

        int totalItems = await _dbContext.Courses.CountAsync(cancellationToken);

        // Fetch courses with pagination
        List<CourseReadModel> courses = await _dbContext.Courses
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Get unique instructor and category IDs
        var instructorIds = courses.Select(c => c.InstructorId).Distinct().ToList();
        var categoryIds = courses.Select(c => c.CategoryId).Distinct().ToList();

        // Fetch related data in batches (composition)
        Dictionary<Guid, InstructorReadModel> instructors = await _dbContext.Instructors
            .Where(i => instructorIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        Dictionary<Guid, CategoryReadModel> categories = await _dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        // Compose DTOs using ReadModelComposer
        var courseDtos = courses.Select(course =>
        {
            instructors.TryGetValue(course.InstructorId, out InstructorReadModel? instructor);
            categories.TryGetValue(course.CategoryId, out CategoryReadModel? category);

            string? thumbnailUrl = course.ImageUrls.Count > 0
                ? _urlResolver.Resolve(StorageCategory.Public, course.ImageUrls[0]).Value
                : null;

            var dto = course.ToCourseSummaryDto(instructor, category, thumbnailUrl);

            // Add links using factory (mutate after creation)
            return dto with
            {
                Links = _courseLinkFactory.CreateLinks(
                    new CourseState(new CourseId(course.Id), new UserId(course.InstructorId), course.Status))
            };
        }).ToList();

        return Result.Success(new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            Links = _courseLinkFactory.CreateCollectionLinks(
                request.PagedQuery with { Page = pageNumber, PageSize = pageSize },
                totalItems)
        });
    }
}
