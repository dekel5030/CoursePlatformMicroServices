using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetFeatured;

public class GetFeaturedQueryHandler : IQueryHandler<GetFeaturedQuery, CourseCollectionDto>
{
    private readonly IFeaturedCoursesRepository _featuredCoursesProvider;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly IReadDbContext _dbContext;

    public GetFeaturedQueryHandler(
        IFeaturedCoursesRepository featuredCoursesProvider,
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext)
    {
        _featuredCoursesProvider = featuredCoursesProvider;
        _urlResolver = urlResolver;
        _dbContext = dbContext;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetFeaturedQuery request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Course> courses = await _featuredCoursesProvider.GetFeaturedCourse();

        Dictionary<UserId, InstructorDto> instructorDtos = await _dbContext.Users
            .Where(u => courses.Select(c => c.InstructorId).Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u =>
                new InstructorDto(
                    u.Id,
                    u.FullName,
                    _urlResolver.Resolve(StorageCategory.Public, u.AvatarUrl ?? "").Value)
            , cancellationToken);

        var courseDtos = courses.Select(course =>
        {
            InstructorDto instructor = instructorDtos.GetValueOrDefault(course.InstructorId)
                         ?? new InstructorDto(course.InstructorId, "Unknown", null);

            return new CourseSummaryDto(
                course.Id,
                course.Title,
                instructor,
                course.Status,
                course.Price,
                course.Images.Select(i => i.Path).FirstOrDefault(),
                course.LessonCount,
                course.EnrollmentCount,
                course.UpdatedAtUtc);

        }).ToList();

        var response = new CourseCollectionDto
        (
            Items: courseDtos,
            PageNumber: 1,
            PageSize: courseDtos.Count,
            TotalItems: courseDtos.Count
        );

        return Result.Success(response);
    }
}
