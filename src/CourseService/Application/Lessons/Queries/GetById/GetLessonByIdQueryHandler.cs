using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions;
using Courses.Application.Actions.Abstract;
using Courses.Application.Lessons.Extensions;
using Courses.Application.Lessons.Queries.Dtos;
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
        Course? course = await _dbContext.Courses
            .Include(c => c.Lessons.Where(l => l.Id == request.LessonId))
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonDetailsDto>(CourseErrors.NotFound);
        }

        Lesson? lesson = course.Lessons.Count > 0 ? course.Lessons[0] : null;
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
            lesson.ThumbnailImageUrl,
            lesson.VideoUrl,
            _actionProvider.GetAllowedActions(course, lesson));

        return Result.Success(response);
    }
}
