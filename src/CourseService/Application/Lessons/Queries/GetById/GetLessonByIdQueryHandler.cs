using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions;
using Courses.Application.Actions.Abstract;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
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
            .Include(lesson => lesson.Course)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsDto>(LessonErrors.NotFound);
        }

        var courseContext = new CoursePolicyContext(
            lesson.CourseId,
            lesson.Course.InstructorId,
            lesson.Course.Status,
            lesson.Course.LessonCount);

        var response = new LessonDetailsDto
        (
            CourseContext: courseContext,
            CourseId: lesson.CourseId,
            LessonId: lesson.Id,
            Title: lesson.Title,
            Description: lesson.Description,
            Index: lesson.Index,
            Duration: lesson.Duration,
            ThumbnailUrl: lesson.ThumbnailImageUrl is not null
                ? _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value
                : null,
            Access: lesson.Access,
            Status: lesson.Status,
            VideoUrl: lesson.VideoUrl is not null
                ? _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl.Path).Value
                : null
        );

        return Result.Success(response);
    }
}
