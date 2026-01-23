using System.Data;
using System.Text.Json;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using SharedDtos = Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;
using Dapper;
using Kernel;
using Kernel.Messaging.Abstractions;
using Courses.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Courses.Domain.Module;
using Courses.Domain.Users;
using Courses.Domain.Categories;
using Courses.Application.Abstractions.Hateoas;
using Courses.Application.Actions;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly IHateoasLinkProvider _hateoasProvider;

    public GetCourseByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext readDbContext,
        IHateoasLinkProvider hateoasProvider)
    {
        _urlResolver = urlResolver;
        _readDbContext = readDbContext;
        _hateoasProvider = hateoasProvider;
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

        var courseContext = new CoursePolicyContext(course.Id, instructor.Id, course.Status, course.LessonCount);

        var moduleDtos = modules.Select(module =>
        {
            return new ModuleDto(
                Id: module.Id.Value,
                Title: module.Title.Value,
                Index: module.Index,
                LessonCount: module.LessonCount,
                Duration: module.Duration,
                Links: _hateoasProvider.CreateModuleCollectionLinks(course.Id),
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
                        Links: _hateoasProvider.CreateLessonLinks(courseContext, new LessonPolicyContext(lesson.Id, lesson.Access), module.Id)
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
            _hateoasProvider.CreateCourseLinks(courseContext));

        return Result<CoursePageDto>.Success(response);
    }
}
