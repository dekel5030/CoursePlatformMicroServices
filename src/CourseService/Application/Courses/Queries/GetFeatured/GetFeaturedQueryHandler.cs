using Courses.Application.Abstractions.Data.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeatured;

public class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseSummaryDto>>
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

    public async Task<Result<PagedResponseDto<CourseSummaryDto>>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        var courses = await _featuredCoursesProvider.GetFeaturedCourse();

        var courseDtos = courses.Select(course => new CourseSummaryDto(
            course.Id.Value,
            course.Title.Value,
            course.InstructorId?.Value.ToString(), 
            course.Price.Amount,
            course.Price.Currency,
            _urlResolver.Resolve(course.Images.FirstOrDefault()?.Path ?? string.Empty),
            course.Lessons.Count,
            course.EnrollmentCount
        )).ToList();

        var response = new PagedResponseDto<CourseSummaryDto>
        {
            Items = courseDtos,
            PageNumber = 1,
            PageSize = Math.Max(1, courseDtos.Count),
            TotalItems = courseDtos.Count
        };

        return Result.Success(response);
    }
}
