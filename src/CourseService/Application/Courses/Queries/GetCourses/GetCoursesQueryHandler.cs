using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCoursesQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.PageNumber ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 100);

        DbSet<Course> baseQuery = _dbContext.Courses;

        int totalItems = await baseQuery.CountAsync(cancellationToken);

        List<Course> coursesData = await _dbContext.Courses
            .Include(c => c.Instructor)
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var courseIds = coursesData.Select(c => c.Id).ToList();
        var modules = await _dbContext.Modules
            .Where(m => courseIds.Contains(m.CourseId))
            .Include(m => m.Lessons)
            .ToListAsync(cancellationToken);

        var modulesByCourse = modules
            .GroupBy(m => m.CourseId)
            .ToDictionary(g => g.Key, g => g.ToList());

        List<CourseSummaryDto> courses = coursesData.Select(course =>
        {
            var courseModules = modulesByCourse.GetValueOrDefault(course.Id, new List<Domain.Module.Module>());
            var lessonCount = courseModules.SelectMany(m => m.Lessons).Count();

            return new CourseSummaryDto(
                course.Id,
                course.Title,
                new InstructorDto(
                    course.InstructorId,
                    $"{course.Instructor!.FirstName} {course.Instructor.LastName}",
                    course.Instructor.AvatarUrl
                ),
                course.Status,
                course.Price,
                course.Images.Select(i => i.Path).FirstOrDefault(),
                lessonCount,
                course.EnrollmentCount,
                course.UpdatedAtUtc
            );
        }).ToList();

        courses = courses.Select(course => course.EnrichWithUrls(_urlResolver)).ToList();

        var response = new CourseCollectionDto(
            courses,
            pageNumber,
            pageSize,
            totalItems);

        return Result.Success(response);
    }
}
