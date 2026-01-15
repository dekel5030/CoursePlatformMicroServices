using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Extensions;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Lessons.Queries.Dtos;
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

    public GetCourseByIdQueryHandler(IReadDbContext dbContext, IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var allowedActions = new List<CourseAction> { CourseAction.Edit, CourseAction.Delete };
        var allowedLessonActions = new List<LessonAction> { LessonAction.Update, LessonAction.Delete };

        Course? course = await _dbContext.Courses
            .Include(c => c.Lessons)
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
            course.InstructorId?.ToString(),
            course.Price.Amount,
            course.Price.Currency,
            course.EnrollmentCount,
            course.UpdatedAtUtc,
            course.Images,
            course.Lessons.Select(lesson => new LessonSummaryDto(
                course.Id,
                lesson.Id,
                lesson.Title,
                lesson.Description,
                lesson.Index,
                lesson.Duration,
                lesson.Access == LessonAccess.Public,
                lesson.ThumbnailImageUrl,
                allowedLessonActions)).ToList(),
            allowedActions);

        return Result.Success(response);
    }
}
