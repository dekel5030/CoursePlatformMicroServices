using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
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

        List<CourseId> courseIds = courses.Select(c => c.Id).ToList();
        // Note: This query assumes modules are already loaded or we need to query them separately
        // For now, using LessonCount from course domain
        var courseDtos = courses.Select(course =>
            new CourseSummaryDto(
                course.Id,
                course.Title,
                new Application.Shared.Dtos.InstructorDto(
                    course.InstructorId,
                    "Unknown", // Instructor may not be loaded
                    null
                ),
                course.Status,
                course.Price,
                course.Images.Select(i => i.Path).FirstOrDefault(),
                course.LessonCount,
                course.EnrollmentCount,
                course.UpdatedAtUtc
            )).ToList();

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
