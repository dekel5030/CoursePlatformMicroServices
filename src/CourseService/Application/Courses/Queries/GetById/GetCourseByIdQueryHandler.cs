using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
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
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(IReadDbContext dbContext, IStorageUrlResolver urlResolver)
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

        var imageTasks = course.Images
            .Select(img => _urlResolver.ResolveAsync(StorageCategory.Public, img.Path))
            .ToList();

        var resolvedImages = await Task.WhenAll(imageTasks);

        var lessonTasks = course.Lessons
            .OrderBy(l => l.Index)
            .Select(async lesson =>
            {
                var thumbnailUrl = lesson.ThumbnailImageUrl != null
                    ? (await _urlResolver.ResolveAsync(StorageCategory.Public, lesson.ThumbnailImageUrl.Path)).Value
                    : string.Empty;

                return new LessonSummaryDto(
                    Id: lesson.Id.Value,
                    Title: lesson.Title.Value,
                    Description: lesson.Description.Value,
                    Index: lesson.Index,
                    Duration: lesson.Duration,
                    IsPreview: lesson.Access == LessonAccess.Public,
                    ThumbnailUrl: thumbnailUrl
                );
            })
            .ToList();

        var lessons = await Task.WhenAll(lessonTasks);

        var response = new CourseDetailsDto(
            Id: course.Id.Value,
            Title: course.Title.Value,
            Description: course.Description.Value,
            InstructorName: course.InstructorId?.Value.ToString(),
            Price: course.Price.Amount,
            Currency: course.Price.Currency,
            EnrollmentCount: course.EnrollmentCount,
            UpdatedAtUtc: course.UpdatedAtUtc,
            ImageUrls: resolvedImages.Select(r => r.Value).ToList(),
            Lessons: lessons.ToList()
        );

        return Result.Success(response);
    }
}