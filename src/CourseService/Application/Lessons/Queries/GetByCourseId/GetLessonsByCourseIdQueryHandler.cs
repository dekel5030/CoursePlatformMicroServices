using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetByCourseId;

internal sealed class GetLessonsByCourseIdQueryHandler
    : IQueryHandler<GetLessonsByCourseIdQuery, IReadOnlyList<LessonDto>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _storageUrlResolver;
    private readonly ILinkBuilderService _linkBuilderService;
    private readonly IMediator _mediator;

    public GetLessonsByCourseIdQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver storageUrlResolver,
        ILinkBuilderService linkBuilderService,
        IMediator mediator)
    {
        _readDbContext = readDbContext;
        _storageUrlResolver = storageUrlResolver;
        _linkBuilderService = linkBuilderService;
        _mediator = mediator;
    }

    public async Task<Result<IReadOnlyList<LessonDto>>> Handle(
        GetLessonsByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Result<CourseDto> courseResult = await _mediator.Send(
            new GetCourseByIdQuery(request.CourseId),
            cancellationToken);

        if (courseResult.IsFailure)
        {
            return Result.Failure<IReadOnlyList<LessonDto>>(courseResult.Error);
        }

        CourseDto courseDto = courseResult.Value;

        List<Lesson> lessons = await _readDbContext.Lessons
            .Where(lesson => lesson.CourseId == request.CourseId)
            .OrderBy(lesson => lesson.ModuleId)
            .ThenBy(lesson => lesson.Index)
            .ToListAsync(cancellationToken);

        var courseContext = new CourseContext(
            request.CourseId,
            new UserId(courseDto.InstructorId),
            courseDto.Status);

        var moduleContexts = lessons
            .Select(l => l.ModuleId)
            .Distinct()
            .ToDictionary(
                mid => mid,
                mid => new ModuleContext(courseContext, mid));

        var response = lessons.Select(lesson =>
        {
            ModuleContext moduleContext = moduleContexts[lesson.ModuleId];

            var lessonContext = new LessonContext(
                moduleContext,
                lesson.Id,
                lesson.Access,
                HasEnrollment: false);

            return new LessonDto
            {
                Id = lesson.Id.Value,
                Title = lesson.Title.Value,
                Index = lesson.Index,
                Duration = lesson.Duration,
                ThumbnailUrl = lesson.ThumbnailImageUrl is null ? null
                    : _storageUrlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value,
                Access = lesson.Access,
                Links = _linkBuilderService.BuildLinks(LinkResourceKey.Lesson, lessonContext).ToList()
            };
        }).ToList();

        return Result.Success<IReadOnlyList<LessonDto>>(response);
    }
}
