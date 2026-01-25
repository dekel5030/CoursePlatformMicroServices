using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
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

        int courseCount = await _dbContext.Courses.CountAsync(cancellationToken);

        List<Course> courses = await _dbContext.Courses
            .OrderByDescending(c => c.UpdatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (courses.Count == 0)
        {
            return Result.Success(new CourseCollectionDto { Items = [], Links = [], PageNumber = 1, PageSize = 0, TotalItems = 0 });
        }

        var instructorIds = courses.Select(c => c.InstructorId).Distinct().ToList();
        var categoryIds = courses.Select(c => c.CategoryId).Distinct().ToList();

        Dictionary<UserId, InstructorDto> instructorsMap = await _dbContext.Users
            .Where(u => instructorIds.Contains(u.Id))
            .ToDictionaryAsync(user => user.Id, user => new InstructorDto(
                user.Id.Value,
                user.FullName,
                user.AvatarUrl == null ? null : _urlResolver.Resolve(StorageCategory.Public, user.AvatarUrl ?? "").Value
            ), cancellationToken);

        Dictionary<CategoryId, CategoryDto> categoriesMap = await _dbContext.Categories
            .Where(c => categoryIds.Select(id => id).Contains(c.Id))
            .ToDictionaryAsync(category => category.Id, category => new CategoryDto(
                category.Id.Value,
                category.Name,
                category.Slug.Value
            ), cancellationToken);

        var ratingsMap = new Dictionary<Guid, (double Avg, int Count)>();

        var courseDtos = courses.Select(course =>
        {
            InstructorDto instructor = instructorsMap.GetValueOrDefault(course.InstructorId)
                             ?? new InstructorDto(course.InstructorId.Value, "Unknown Instructor", null);

            CategoryDto category = categoriesMap.GetValueOrDefault(course.CategoryId)
                           ?? new CategoryDto(Guid.Empty, "Uncategorized", "uncategorized");

            ImageUrl? mainImage = course.Images.FirstOrDefault();
            string? thumbnailUrl = mainImage is not null
                ? _urlResolver.Resolve(StorageCategory.Public, mainImage.Path).Value
                : null;

            string descriptionText = course.Description.Value;
            string shortDescription = descriptionText.Length > 50
                ? descriptionText[..50] + "..."
                : descriptionText;

            var badges = new List<string>();
            if (course.EnrollmentCount > 1000)
            {
                badges.Add("Bestseller");
            }

            if (course.UpdatedAtUtc > DateTimeOffset.UtcNow.AddDays(-30))
            {
                badges.Add("New");
            }

            (double avg, int count) = ratingsMap.GetValueOrDefault(course.Id.Value, (Avg: 0, Count: 0));

            return new CourseSummaryDto
            {
                Id = course.Id.Value,
                Title = course.Title.Value,
                Slug = course.Slug.Value,
                ShortDescription = shortDescription,

                Instructor = instructor,
                Category = category,

                Price = course.Price,
                OriginalPrice = null,
                Badges = badges,

                AverageRating = avg,
                ReviewsCount = count,

                ThumbnailUrl = thumbnailUrl,
                LessonsCount = course.LessonCount,
                Duration = course.Duration,
                Difficulty = course.Difficulty,

                EnrollmentCount = course.EnrollmentCount,
                CourseViews = course.Views,
                UpdatedAtUtc = course.UpdatedAtUtc,

                Status = course.Status,
                Links = _courseLinkFactory.CreateLinks(new CourseState(course.Id, course.InstructorId, course.Status, course.LessonCount))
            };
        }).ToList();

        var response = new CourseCollectionDto
        {
            Items = courseDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = courseCount,
            Links = _courseLinkFactory.CreateCollectionLinks(request.PagedQuery with { Page = pageNumber, PageSize = pageSize }, courseCount)
        };

        return Result.Success(response);
    }
}
