using System.Data;
using System.Linq;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.LinkProvider;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions.Courses;
using Courses.Application.Actions.Lessons;
using Courses.Application.Actions.Modules;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ICourseLinkProvider _courseLinkProvider;
    private readonly IModuleLinkProvider _moduleLinkProvider;
    private readonly ILessonLinkProvider _lessonLinkProvider;

    public GetCourseByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext readDbContext,
        ICourseLinkProvider courseLinkProvider,
        IModuleLinkProvider moduleLinkProvider,
        ILessonLinkProvider lessonLinkProvider)
    {
        _urlResolver = urlResolver;
        _readDbContext = readDbContext;
        _courseLinkProvider = courseLinkProvider;
        _moduleLinkProvider = moduleLinkProvider;
        _lessonLinkProvider = lessonLinkProvider;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _readDbContext.Courses
            .Where(c => c.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Result<CoursePageDto>.Failure(CourseErrors.NotFound);
        }

        User instructor = await _readDbContext.Users
            .Where(u => u.Id == course.InstructorId)
            .FirstAsync(cancellationToken);

        List<Module> modules = await _readDbContext.Modules
            .Include(m => m.Lessons)
            .Where(m => m.CourseId == request.Id)
            .ToListAsync(cancellationToken);

        Category category = await _readDbContext.Categories
            .Where(c => c.Id == course.CategoryId)
            .FirstAsync(cancellationToken);

        var images = course.Images
            .Select(image => _urlResolver.Resolve(StorageCategory.Public, image.Path).Value)
            .ToList();

        var tags = course.Tags.Select(tag => tag.Value).ToList();

        var courseState = new CourseState(course.Id, instructor.Id, course.Status, course.LessonCount);

        var moduleDtos = modules.Select(module =>
        {
            return new ModuleDto(
                Id: module.Id.Value,
                Title: module.Title.Value,
                Index: module.Index,
                LessonCount: module.LessonCount,
                Duration: module.Duration,
                Links: _moduleLinkProvider.CreateLinks(new ModuleState(module.CourseId, module.Id)).ToList(),
                Lessons: module.Lessons.Select(lesson =>
                {
                    return new LessonDto(
                        LessonId: lesson.Id.Value,
                        Title: lesson.Title.Value,
                        Index: lesson.Index,
                        Duration: lesson.Duration,
                        ThumbnailUrl: lesson.ThumbnailImageUrl == null ? null :
                            _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl.Path).Value,
                        Access: lesson.Access.ToString(),
                        Links: _lessonLinkProvider.CreateLinks(new LessonState(lesson.Id)).ToList()
                    );
                }).ToList()
            );
        }).ToList();

        var response = new CoursePageDto(
            course.Id.Value,
            course.Title.Value,
            course.Description.Value,
            instructor.Id.Value,
            instructor.FullName,
            instructor.AvatarUrl,
            course.Status,
            course.Price,
            course.EnrollmentCount,
            course.LessonCount,
            course.Duration,
            course.UpdatedAtUtc,
            images,
            tags,
            category.Id.Value,
            category.Name,
            category.Slug.Value,
            moduleDtos,
            _courseLinkProvider.CreateLinks(courseState).ToList());

        return Result<CoursePageDto>.Success(response);
    }
}
