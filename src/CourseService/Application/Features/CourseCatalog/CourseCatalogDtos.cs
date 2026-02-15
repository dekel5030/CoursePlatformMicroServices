using Courses.Application.Categories.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Features.CourseCatalog;

public sealed record CourseCatalogDto(
    IReadOnlyList<CourseCatalogItemDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    CourseCatalogCollectionLinks Links);

public sealed record CourseCatalogItemDto(
    CourseCatalogItemData Data,
    CourseCatalogItemLinks Links);

public sealed record CourseCatalogItemData(
    Guid Id,
    string Title,
    string ShortDescription,
    string Slug,
    UserDto Instructor,
    CategoryDto Category,
    Money Price,
    DifficultyLevel Difficulty,
    string? ThumbnailUrl,
    DateTimeOffset UpdatedAtUtc,
    CourseStatus Status,
    int LessonsCount,
    TimeSpan Duration,
    int EnrollmentCount,
    double AverageRating,
    int ReviewsCount,
    int CourseViews);

public sealed record CourseCatalogItemLinks(LinkRecord Self);

public sealed record CourseCatalogCollectionLinks(LinkRecord Self);
