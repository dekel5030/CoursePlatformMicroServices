using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions.Abstract;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
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
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IStorageUrlResolver _urlResolver;
#pragma warning restore S4487 // Unread "private" fields should be removed
    private readonly ICourseActionProvider _actionProvider;

    public GetLessonByIdQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver,
        ICourseActionProvider actionProvider)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
        _actionProvider = actionProvider;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _dbContext.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => 
                l.Id == request.LessonId && 
                l.CourseId == request.CourseId, 
                cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsDto>(LessonErrors.NotFound);
        }

        LessonDetailsDto response = new(
            lesson.CourseId,
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Index,
            lesson.Duration,
            lesson.Access == LessonAccess.Public,
            lesson.ThumbnailImageUrl == null ? null : _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value,
            lesson.VideoUrl == null ? null : _urlResolver.Resolve(StorageCategory.Private, lesson.VideoUrl.Path).Value,
            _actionProvider.GetAllowedActions(lesson.Course, lesson));

        return Result.Success(response);
    }
}
