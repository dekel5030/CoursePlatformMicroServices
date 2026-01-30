using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses.Errors;
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
        CourseReadModel? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        InstructorReadModel? instructor = await _readDbContext.Instructors
            .FirstOrDefaultAsync(i => i.Id == course.InstructorId, cancellationToken);

        CategoryReadModel? category = await _readDbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == course.CategoryId, cancellationToken);

        List<ModuleReadModel> modules = await _readDbContext.Modules
            .Where(m => m.CourseId == request.Id)
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);

        var moduleIds = modules.Select(m => m.Id).ToList();
        List<LessonReadModel> lessons = await _readDbContext.Lessons
            .Where(l => moduleIds.Contains(l.ModuleId))
            .OrderBy(l => l.Index)
            .ToListAsync(cancellationToken);

        var lessonsByModule = lessons
            .GroupBy(l => l.ModuleId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var moduleDtos = modules.Select(module =>
        {
            lessonsByModule.TryGetValue(module.Id, out List<LessonReadModel>? moduleLessons);
            moduleLessons ??= [];

            return new ModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                Index = module.Index,
                Duration = TimeSpan.FromSeconds(module.TotalDurationSeconds),
                LessonCount = module.LessonCount,
                Lessons = moduleLessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Index = l.Index,
                    Duration = l.Duration,
                    ThumbnailUrl = l.ThumbnailUrl,
                    Access = l.Access,
                    Links = []
                }).ToList(),
                Links = []
            };
        }).ToList();

        var resolvedImageUrls = course.ImageUrls
            .Select(url => _urlResolver.Resolve(StorageCategory.Public, url).Value)
            .ToList();

        var pageDto = new CoursePageDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            InstructorName = instructor?.FullName ?? "Unknown",
            InstructorAvatarUrl = instructor?.AvatarUrl,
            Status = course.Status,
            Price = new Money(course.PriceAmount, course.PriceCurrency),
            EnrollmentCount = course.EnrollmentCount,
            LessonsCount = course.TotalLessons,
            TotalDuration = TimeSpan.FromSeconds(course.TotalDurationSeconds),
            UpdatedAtUtc = course.UpdatedAtUtc,
            ImageUrls = resolvedImageUrls.AsReadOnly(),
            Tags = course.Tags.AsReadOnly(),
            CategoryId = course.CategoryId,
            CategoryName = category?.Name ?? "Unknown",
            CategorySlug = category?.Slug ?? string.Empty,
            Modules = moduleDtos,
            Links = []
        };

        return Result.Success(pageDto);
    }
}
