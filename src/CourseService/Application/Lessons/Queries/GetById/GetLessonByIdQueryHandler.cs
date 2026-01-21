using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Primitives;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

public class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetLessonByIdQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _dbContext.Lessons
            .Where(lesson => lesson.Id == request.LessonId)
            .Include(lesson => lesson.Module)
                .ThenInclude(module => module.Course)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsDto>(LessonErrors.NotFound);
        }

        if (lesson.Module is null || lesson.Module.Course is null)
        {
            return Result.Failure<LessonDetailsDto>(
                Error.NotFound("Module.NotFound", "The module or course for this lesson was not found."));
        }

        var course = lesson.Module.Course;
        var courseContext = new CoursePolicyContext(
            course.Id,
            course.InstructorId,
            course.Status,
            course.LessonCount);

        var response = new LessonDetailsDto
        (
            CourseContext: courseContext,
            CourseId: course.Id,
            ModuleId: lesson.ModuleId,
            LessonId: lesson.Id,
            Title: lesson.Title,
            Description: lesson.Description,
            Index: lesson.Index,
            Duration: lesson.Duration,
            ThumbnailUrl: lesson.ThumbnailImageUrl is not null
                ? _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value
                : null,
            Access: lesson.Access,
            Status: course.Status == Domain.Courses.Primitives.CourseStatus.Published 
                ? LessonStatus.Published 
                : LessonStatus.Draft,
            VideoUrl: lesson.VideoUrl is not null
                ? _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl.Path).Value
                : null
        );

        return Result.Success(response);
    }
}
