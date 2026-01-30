using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCoursePage;

internal sealed class GetCoursePageQueryHandler
    : IQueryHandler<GetCoursePageQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCoursePageQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        CourseId requestedId = new(request.Id);
        Course? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(course => course.Id == requestedId, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        User? instructor = await _readDbContext.Users
            .FirstOrDefaultAsync(i => i.Id == course.InstructorId, cancellationToken);

        Category? category = await _readDbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == course.CategoryId, cancellationToken);

        List<Module> modules = await _readDbContext.Modules
            .Where(m => m.CourseId == course.Id)
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);

        var moduleIds = modules.Select(m => m.Id).ToList();
        List<Lesson> lessonsGroupedByModule = await _readDbContext.Lessons
            .Where(l => moduleIds.Contains(l.ModuleId))
            .OrderBy(l => l.Index)
            .ToListAsync(cancellationToken);

        var lessonsByModuleId = lessonsGroupedByModule
            .GroupBy(l => l.ModuleId)
            .ToDictionary(g => g.Key, g => g.ToList());

        int enrollmentCount = await _readDbContext.Enrollments
            .CountAsync(e => e.CourseId == course.Id, cancellationToken);

        var moduleDtos = modules.Select(module =>
        {
            List<Lesson> moduleLessons = lessonsByModuleId.TryGetValue(module.Id, out List<Lesson>? lessons)
                ? lessons
                : [];

            var lessonDtos = moduleLessons
            .Select(l => new LessonDto
            {
                Id = l.Id.Value,
                Title = l.Title.Value,
                Index = l.Index,
                Duration = l.Duration,
                ThumbnailUrl = l.ThumbnailImageUrl != null
                    ? _urlResolver.Resolve(StorageCategory.Public, l.ThumbnailImageUrl.Path).Value
                    : null,
                Access = l.Access,
                Links = []
            }).ToList();

            return new ModuleDto
            {
                Id = module.Id.Value,
                Title = module.Title.Value,
                Index = module.Index,
                Duration = TimeSpan.FromSeconds(moduleLessons.Sum(l => l.Duration.TotalSeconds)),
                LessonCount = moduleLessons.Count,
                Lessons = lessonDtos,
                Links = []
            };
        }).ToList();

        var resolvedImageUrls = course.Images
            .Select(img => _urlResolver.Resolve(StorageCategory.Public, img.Path).Value)
            .ToList();

        int totalLessons = lessonsGroupedByModule.Count;
        double totalDurationSeconds = lessonsGroupedByModule.Sum(l => l.Duration.TotalSeconds);

        var pageDto = new CoursePageDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            Description = course.Description.Value,
            InstructorId = course.InstructorId.Value,
            InstructorName = instructor?.FullName ?? "Unknown",
            InstructorAvatarUrl = instructor?.AvatarUrl,
            Status = course.Status,
            Price = course.Price,
            EnrollmentCount = enrollmentCount,
            LessonsCount = totalLessons,
            TotalDuration = TimeSpan.FromSeconds(totalDurationSeconds),
            UpdatedAtUtc = course.UpdatedAtUtc,
            ImageUrls = resolvedImageUrls.AsReadOnly(),
            Tags = course.Tags.Select(t => t.Value).ToList().AsReadOnly(),
            CategoryId = course.CategoryId.Value,
            CategoryName = category?.Name ?? "Unknown",
            CategorySlug = category?.Slug.Value ?? string.Empty,
            Modules = moduleDtos,
            Links = []
        };

        return Result.Success(pageDto);
    }
}
