using Courses.Application.Abstractions;
using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(IReadDbContext dbContext, IUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        CourseId courseId = new(request.Id);

        var course = await _dbContext.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseDetailsDto>(CourseErrors.NotFound);
        }

        var response = new CourseDetailsDto
        (
            Id: course.Id.Value,
            Title: course.Title.Value,
            Description: course.Description.Value,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            EnrollmentCount: course.EnrollmentCount,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: course.Images
                .Select(img => _urlResolver.Resolve(img.Path))
                .ToList(),
            Lessons: course.Lessons
                .OrderBy(l => l.Index)
                .Select(lesson => new LessonSummaryDto
                (
                    Id: lesson.Id.Value,
                    Title: lesson.Title.Value,
                    Description: lesson.Description.Value,
                    Index: lesson.Index,
                    Duration: lesson.Duration,
                    IsPreview: lesson.Access == LessonAccess.Public,
                    ThumbnailUrl: _urlResolver.Resolve(lesson.ThumbnailImageUrl?.Path ?? string.Empty)
                )).ToList()
        );

        return Result.Success(response);
    }
}
