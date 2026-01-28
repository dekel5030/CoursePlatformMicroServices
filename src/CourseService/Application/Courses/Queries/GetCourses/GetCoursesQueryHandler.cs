using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ICourseLinkFactory _courseLinkFactory;

    public GetCoursesQueryHandler(
        IStorageUrlResolver urlResolver,
        IReadDbContext dbContext,
        ICourseLinkFactory courseLinkFactory)
    {
        _urlResolver = urlResolver;
        _dbContext = dbContext;
        _courseLinkFactory = courseLinkFactory;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.Page ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 25);

        int totalItems = await _dbContext.Courses.CountAsync(cancellationToken);

        var rawData = await _dbContext.Courses
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                Course = c,
                Instructor = _dbContext.Users
                    .Where(u => u.Id == c.InstructorId)
                    .Select(u => new { u.Id, u.FullName, u.AvatarUrl })
                    .FirstOrDefault(),
                Category = _dbContext.Categories
                    .Where(cat => cat.Id == c.CategoryId)
                    .Select(cat => new { cat.Id, cat.Name, cat.Slug })
                    .FirstOrDefault(),

                EnrollmentCount = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),

                LessonsCount = _dbContext.Modules
                    .Where(m => m.CourseId == c.Id)
                    .SelectMany(m => _dbContext.Lessons.Where(l => l.ModuleId == m.Id))
                    .Count(),

                TotalDurationSeconds = _dbContext.Modules
                    .Where(m => m.CourseId == c.Id)
                    .SelectMany(m => _dbContext.Lessons.Where(l => l.ModuleId == m.Id))
                    .Sum(l => l.Duration.TotalSeconds)
            })
            .ToListAsync(cancellationToken);

        var courseDtos = rawData.Select(data =>
        {
            Course course = data.Course;

            string descriptionText = course.Description.Value;
            string shortDescription = descriptionText.Length > 100
                ? descriptionText[..100] + "..."
                : descriptionText;

            string? thumbnailUrl = course.Images.Count != 0
                ? _urlResolver.Resolve(StorageCategory.Public, course.Images.First().Path).Value
                : null;

            string? avatarUrl = data.Instructor?.AvatarUrl != null
                ? _urlResolver.Resolve(StorageCategory.Public, data.Instructor.AvatarUrl).Value
                : null;

            return new CourseSummaryDto
            {
                Id = course.Id.Value,
                Title = course.Title.Value,
                Slug = course.Slug.Value,
                ShortDescription = shortDescription,
                Price = course.Price,
                OriginalPrice = course.Price,
                Difficulty = course.Difficulty,
                Status = course.Status,
                UpdatedAtUtc = course.UpdatedAtUtc,

                EnrollmentCount = data.EnrollmentCount,
                LessonsCount = data.LessonsCount,
                Duration = TimeSpan.FromSeconds(data.TotalDurationSeconds),

                AverageRating = 4.5,
                ReviewsCount = 120,
                CourseViews = 1245,

                Instructor = new InstructorDto(
                    data.Instructor?.Id.Value ?? Guid.Empty,
                    data.Instructor?.FullName ?? "Unknown",
                    avatarUrl),

                Category = new CategoryDto(
                    data.Category?.Id.Value ?? Guid.Empty,
                    data.Category?.Name ?? "Uncategorized",
                    data.Category?.Slug.Value ?? "uncategorized"),

                ThumbnailUrl = thumbnailUrl,
                Badges = course.UpdatedAtUtc > DateTimeOffset.UtcNow.AddDays(-30) ? new List<string> { "New" } : [],

                Links = _courseLinkFactory.CreateLinks(new CourseState(course.Id, course.InstructorId, course.Status))
            };
        }).ToList();

        return Result.Success(new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            Links = _courseLinkFactory.CreateCollectionLinks(request.PagedQuery with { Page = pageNumber, PageSize = pageSize }, totalItems)
        });
    }
}
