using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeatured;

public class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, CourseCollectionDto>
{
    private readonly IFeaturedCoursesRepository _featuredCoursesProvider;
    private readonly IStorageUrlResolver _urlResolver;

    public GetFeaturedQueryHandler(
        IFeaturedCoursesRepository featuredCoursesProvider,
        IStorageUrlResolver urlResolver)
    {
        _featuredCoursesProvider = featuredCoursesProvider;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Course> courses = await _featuredCoursesProvider.GetFeaturedCourse();

        var courseDtos = courses
            .AsQueryable()
            .Select(ProjectionMappings.ToCourseSummary)
            .ToList();

        var response = new CourseCollectionDto
        (
            Items: courseDtos,
            PageNumber: 1,
            PageSize: courseDtos.Count,
            TotalItems: courseDtos.Count
        );

        return Result.Success(response.EnrichWithUrls(_urlResolver));
    }
}
