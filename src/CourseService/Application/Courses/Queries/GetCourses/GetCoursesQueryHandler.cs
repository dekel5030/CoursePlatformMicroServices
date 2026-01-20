using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions.Abstract;
using Courses.Application.Courses.Dtos;
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

        List<Course> courses = await _dbContext.Courses
            .Include(course => course.Instructor)
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var courseDtos = courses
            .Select(course => new CourseSummaryDto(
                course.Id,
                course.Title,
                course.Instructor?.FullName ?? "Unknown Instructor",
                course.Price.Amount,
                course.Price.Currency,
                course.Images.Count <= 0 ? null : _urlResolver.Resolve(StorageCategory.Public, course.Images[^1].Path).Value,
                course.LessonCount,
                course.EnrollmentCount))
            .ToList();

        var response = new CourseCollectionDto(
            courseDtos,
            pageNumber,
            pageSize,
            totalItems);

        return Result.Success(response);
    }
}
