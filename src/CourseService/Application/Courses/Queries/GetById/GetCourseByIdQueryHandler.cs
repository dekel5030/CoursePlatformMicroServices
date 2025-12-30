using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

public interface IUrlResolver
{
    string Resolve(string relativePath);
}

internal class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseReadDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(IReadDbContext dbContext, IUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseReadDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        CourseId courseId = new(request.Id);

        var course = await _dbContext.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseReadDto>(CourseErrors.NotFound);
        }

        var response = new CourseReadDto
        (
            Id: course.Id.Value,
            Title: course.Title.Value,
            Description: course.Description.Value,
            InstructorId: course.InstructorId?.Value,
            UpdatedAtUtc: course.UpdatedAtUtc,
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            EnrollmentCount: course.EnrollmentCount,
            ImagesUrls: course.Images
                .Select(img => _urlResolver.Resolve(img.Path))
                .ToList(),
            Lessons: course.Lessons
                .OrderBy(l => l.Index)
                .Select(lesson => new LessonReadDto
                (
                    Id: lesson.Id.Value,
                    Title: lesson.Title.Value,
                    Description: lesson.Description.Value,
                    Access: lesson.Access,
                    Status: lesson.Status,
                    Index: lesson.Index,
                    ThumbnailImageUrl: _urlResolver.Resolve(lesson.ThumbnailImageUrl?.Path ?? string.Empty),
                    VideoUrl: null,
                    Duration: lesson.Duration
                )).ToList()
        );

        return Result.Success(response);
    }
}