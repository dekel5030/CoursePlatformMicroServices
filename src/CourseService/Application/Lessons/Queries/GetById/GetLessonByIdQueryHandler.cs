using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Hateoas;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

internal sealed class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsPageDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly IHateoasLinkProvider _hateoasProvider;

    public GetLessonByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        IHateoasLinkProvider hateoasProvider)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _hateoasProvider = hateoasProvider;
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

        User instructor = await _dbContext.Users
            .Where(u => u.Id == course.InstructorId)
            .FirstAsync(cancellationToken);

        // Create policy contexts for link generation
        var courseContext = new CoursePolicyContext(course.Id, instructor.Id, course.Status, course.LessonCount);
        var lessonContext = new LessonPolicyContext(lesson.Id, lesson.Access);

        // Generate links
        IReadOnlyCollection<LinkDto> links = _hateoasProvider.CreateLessonLinks(courseContext, lessonContext, lesson.ModuleId);

        var lessonDetailsPageDto = new LessonDetailsPageDto(
            lesson.Id,
            lesson.ModuleId,
            course.Id,
            course.Title.Value,
            lesson.Title,
            lesson.Description,
            lesson.Index,
            lesson.Duration,
            _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl?.Path ?? "").Value,
            lesson.Access.ToString(),
            _urlResolver.Resolve(StorageCategory.Public, lesson.VideoUrl?.Path ?? "").Value,
            links);

        return Result.Success(lessonDetailsPageDto);
    }
}
