using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Module;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

internal sealed class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsPageDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ILessonLinkFactory _lessonLinkFactory;

    public GetLessonByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        ILessonLinkFactory lessonLinkFactory)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _lessonLinkFactory = lessonLinkFactory;
    }

    public async Task<Result<LessonDetailsPageDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _dbContext.Lessons
            .Where(lesson => lesson.Id == request.LessonId)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        Module? module = await _dbContext.Modules
            .Where(m => m.Id == lesson.ModuleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (module is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _dbContext.Courses
            .Where(c => c.Id == module.CourseId)
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound);
        }

        var courseState = new CourseState(course.Id, course.InstructorId, course.Status, course.LessonCount);
        var moduleState = new ModuleState(module.Id);
        var lessonState = new LessonState(lesson.Id, lesson.Access);

        var lessonDetailsPageDto = new LessonDetailsPageDto
        {
            LessonId = lesson.Id.Value,
            ModuleId = lesson.ModuleId.Value,
            CourseId = course.Id.Value,
            CourseName = course.Title.Value,
            Title = lesson.Title.Value,
            Description = lesson.Description.Value,
            Index = lesson.Index,
            Duration = lesson.Duration,
            ThumbnailUrl = lesson.ThumbnailImageUrl == null ? null : _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value,
            Access = lesson.Access,
            VideoUrl = lesson.VideoUrl == null ? null : _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl.Path).Value,
            TranscriptUrl = lesson.Transcript == null ? null : _urlResolver.Resolve(StorageCategory.Public, lesson.Transcript.Path).Value,
            Links = _lessonLinkFactory.CreateLinks(courseState, moduleState, lessonState)
        };

        return Result.Success(lessonDetailsPageDto);
    }
}
