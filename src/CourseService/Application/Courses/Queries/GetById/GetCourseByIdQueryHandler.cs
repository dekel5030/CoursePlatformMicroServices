using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Primitives;
using Courses.Application.Shared.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _dbContext.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseDetailsDto>(CourseErrors.NotFound);
        }

        var modules = await _dbContext.Modules
            .Where(m => m.CourseId == course.Id)
            .Include(m => m.Lessons)
            .Include(m => m.Course)
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);

        var allLessons = modules
            .SelectMany(m => m.Lessons.OrderBy(l => l.Index))
            .Select((lesson, index) => new { Lesson = lesson, Module = modules.First(m => m.Lessons.Contains(lesson)) })
            .Select(x => new LessonSummaryDto(
                x.Module.CourseId,
                x.Module.Id,
                x.Lesson.Id,
                x.Lesson.Title,
                x.Lesson.Index,
                x.Lesson.Duration,
                x.Lesson.ThumbnailImageUrl != null ? x.Lesson.ThumbnailImageUrl.Path : null,
                course.Status == Domain.Courses.Primitives.CourseStatus.Published
                    ? Application.Lessons.Primitives.LessonStatus.Published
                    : Application.Lessons.Primitives.LessonStatus.Draft,
                x.Lesson.Access))
            .ToList();

        var courseDto = new CourseDetailsDto(
            course.Id,
            course.Title,
            course.Description,
            new InstructorDto(
                course.InstructorId,
                $"{course.Instructor!.FirstName} {course.Instructor.LastName}",
                course.Instructor.AvatarUrl
            ),
            course.Status,
            course.Price,
            course.EnrollmentCount,
            allLessons.Count,
            course.UpdatedAtUtc,
            course.Images.Select(i => i.Path).ToList(),
            allLessons
        );

        CourseDetailsDto response = courseDto.EnrichWithUrls(_urlResolver);

        return Result.Success(response);
    }
}
