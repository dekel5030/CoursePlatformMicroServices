using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Extensions;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
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
        IReadOnlyList<Course> courses = await _featuredCoursesProvider.GetFeaturedCourse();

        List<CourseSummaryDto> courseDtos = await courses.ToSummaryDtosAsync(_urlResolver, cancellationToken);

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
