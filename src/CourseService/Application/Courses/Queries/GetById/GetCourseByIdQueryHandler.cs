using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions.Abstract;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDetailsDto>
{
    private readonly IReadDbContext _dbContext;
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IStorageUrlResolver _urlResolver;
#pragma warning restore S4487 // Unread "private" fields should be removed
    private readonly ICourseActionProvider _courseActionProvider;

    public GetCourseByIdQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver,
        ICourseActionProvider courseActionProvider)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
        _courseActionProvider = courseActionProvider;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _dbContext.Courses
            .Include(c => c.Lessons)
            .Include(c => c.Instructor)
            .Where(c => c.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseDetailsDto>(CourseErrors.NotFound);
        }

        CourseDetailsDto response = new(
            course.Id,
            course.Title,
            course.Description,
            course.Instructor?.FullName ?? "Unknown Instructor",
            course.Price.Amount,
            course.Price.Currency,
            course.EnrollmentCount,
            course.UpdatedAtUtc,
            course.Images.Select(image => _urlResolver.Resolve(StorageCategory.Public, image.Path).Value).Reverse().ToList(),
            AllowedActions: _courseActionProvider.GetAllowedActions(course),
            Lessons: course.Lessons
                .Select(lesson => new LessonSummaryDto(
                    course.Id,
                    lesson.Id,
                    lesson.Title,
                    lesson.Description,
                    lesson.Index,
                    lesson.Duration,
                    lesson.Access == LessonAccess.Public,
                    lesson.ThumbnailImageUrl == null ? null
                        : _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value,
                    _courseActionProvider.GetAllowedActions(course, lesson)))
                .ToList()
        );

        return Result.Success(response);
    }
}
