using Courses.Application.Abstractions;
using Courses.Application.Abstractions.Data.Repositories;
using Courses.Application.Courses.Queries.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetFeatured;

public class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>>
{
    private readonly IFeaturedCoursesRepository _featuredCoursesProvider;
    private readonly IUrlResolver _urlResolver;

    public GetFeaturedQueryHandler(
        IFeaturedCoursesRepository featuredCoursesProvider,
        IUrlResolver urlResolver)
    {
        _featuredCoursesProvider = featuredCoursesProvider;
        _urlResolver = urlResolver;
    }

    public async Task<Result<PagedResponseDto<CourseReadDto>>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        var courses = await _featuredCoursesProvider.GetFeaturedCourse();

        var courseDtos = courses.Select(course => new CourseReadDto(
            course.Id.Value,
            course.Title.Value,
            course.Description.Value,
            course.InstructorId?.Value,
            course.Price.Amount,
            course.Price.Currency,
            course.EnrollmentCount,
            course.UpdatedAtUtc,
            course.Images
                .Select(img => _urlResolver.Resolve(img.Path))
                .ToList(),
            course.Lessons
                .OrderBy(l => l.Index)
                .Select(lesson => new LessonReadDto(
                    lesson.Id.Value,
                    lesson.Title.Value,
                    lesson.Description.Value,
                    lesson.Access,
                    lesson.Status,
                    lesson.Index,
                    _urlResolver.Resolve(lesson.ThumbnailImageUrl?.Path ?? string.Empty),
                    null,
                    lesson.Duration))
                .ToList()
        )).ToList();

        var response = new PagedResponseDto<CourseReadDto>
        {
            Items = courseDtos,
            PageNumber = 1,
            PageSize = Math.Max(1, courseDtos.Count),
            TotalItems = courseDtos.Count
        };

        return Result.Success(response);
    }
}