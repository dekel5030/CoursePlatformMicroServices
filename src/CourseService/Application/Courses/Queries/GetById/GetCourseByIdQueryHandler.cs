using System.Data;
using System.Text.Json;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
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
using Courses.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Courses.Domain.Courses;
using Courses.Domain.Module;
using Courses.Domain.Users;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext readDbContext)
    {
        _urlResolver = urlResolver;
        _readDbContext = readDbContext;
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

        CategoryDto categoryDto = await _readDbContext.Categories
            .Where(category => category.Id == course.CategoryId)
            .Select(category => new CategoryDto(category.Id, category.Name, category.Slug))
            .FirstAsync(cancellationToken);

        var instructorDto = new InstructorDto(
            instructor.Id,
            instructor.FullName,
            instructor.AvatarUrl);

        var images = course.Images
            .Select(image => _urlResolver.Resolve(StorageCategory.Public, image.Path).Value)
            .ToList();

        var tags = course.Tags.Select(tag => new TagDto(tag.Value)).ToList();

        var lessonDtos = modules
            .SelectMany(module => module.Lessons)
            .Select(lesson => new LessonSummaryDto(
                lesson.ModuleId,
                lesson.Id,
                lesson.Title,
                lesson.Index,
                lesson.Duration,
                _urlResolver.Resolve(StorageCategory.Public, lesson.ThumbnailImageUrl?.Path ?? "").Value,
                lesson.Access))
            .ToList();

        var moduleDtos = modules.Select(module => new ModuleDetailsDto(
            module.Id,
            module.Title,
            module.Index,
            module.LessonCount,
            module.Duration,
            lessonDtos)

        ).ToList();

        var response = new CoursePageDto(
            course.Id,
            course.Title,
            course.Description,
            instructorDto,
            course.Status,
            course.Price,
            course.EnrollmentCount,
            course.LessonCount,
            course.Duration,
            course.UpdatedAtUtc,
            images,
            tags,
            categoryDto,
            moduleDtos);

        return Result<CoursePageDto>.Success(response);
    }
}
