using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCoursesQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.PageNumber ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 100);

        int courseCount = await _dbContext.Courses.CountAsync(cancellationToken);
        List<Course> courses = await _dbContext.Courses
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

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
                _urlResolver.Resolve(StorageCategory.Public, course.Images.Select(i => i.Path).FirstOrDefault() ?? "").Value,
                course.LessonCount,
                course.EnrollmentCount,
                course.UpdatedAtUtc);

        }).ToList();

        var response = new CourseCollectionDto
        (
            Items: courseDtos,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalItems: courseCount
        );

        return Result.Success(response);
    }
}
