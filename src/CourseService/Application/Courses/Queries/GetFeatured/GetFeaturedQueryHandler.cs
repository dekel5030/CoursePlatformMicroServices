using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Shared.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetFeatured;

internal sealed class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, CourseCollectionDto>
{
    private readonly IFeaturedCoursesRepository _featuredCoursesRepo;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly IReadDbContext _dbContext;
    private readonly ICourseLinkFactory _courseLinkFactory;

    public GetFeaturedQueryHandler(
        IFeaturedCoursesRepository featuredCoursesProvider,
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        ICourseLinkFactory courseLinkFactory)
    {
        _featuredCoursesRepo = featuredCoursesProvider;
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _courseLinkFactory = courseLinkFactory;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CourseId> featuredCourseIds = await _featuredCoursesRepo.GetFeaturedCourseIds();
        var courseIds = featuredCourseIds.Select(c => c.Value).ToList();

        List<CourseReadModel> courses = await _dbContext.Courses
            .Where(course => courseIds.Contains(course.Id))
            .OrderByDescending(c => c.UpdatedAtUtc)
            .ToListAsync(cancellationToken);

        var instructorIds = courses.Select(c => c.InstructorId).Distinct().ToList();
        var categoryIds = courses.Select(c => c.CategoryId).Distinct().ToList();

        Dictionary<Guid, InstructorReadModel> instructors = await _dbContext.Instructors
            .Where(i => instructorIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        Dictionary<Guid, CategoryReadModel> categories = await _dbContext.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var courseDtos = courses.Select(course =>
        {
            instructors.TryGetValue(course.InstructorId, out InstructorReadModel? instructor);
            categories.TryGetValue(course.CategoryId, out CategoryReadModel? category);

            string? thumbnailUrl = course.ImageUrls.Count > 0
                ? _urlResolver.Resolve(StorageCategory.Public, course.ImageUrls[0]).Value
                : null;

            var dto = course.ToCourseSummaryDto(instructor, category, thumbnailUrl);

            return dto with
            {
                Links = _courseLinkFactory.CreateLinks(
                    new CourseState(new CourseId(course.Id), new UserId(course.InstructorId), course.Status))
            };
        }).ToList();

        var response = new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = 1,
            PageSize = courseDtos.Count,
            TotalItems = courseDtos.Count,
            Links = _courseLinkFactory.CreateCollectionLinks(new PagedQueryDto { Page = 1, PageSize = courseDtos.Count }, courseDtos.Count)
        };

        return Result.Success(response);
    }
}
