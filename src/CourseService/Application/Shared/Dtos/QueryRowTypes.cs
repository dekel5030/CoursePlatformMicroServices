using Courses.Application.Lessons.Primitives;

namespace Courses.Application.Shared.Dtos;

// Shared row types for Dapper queries across the application layer

public sealed record CourseRow(
    Guid Id,
    string Title,
    string Description,
    string Status,
    decimal PriceAmount,
    string PriceCurrency,
    int EnrollmentCount,
    int LessonCount,
    TimeSpan TotalDuration,
    DateTimeOffset UpdatedAtUtc,
    Guid InstructorId,
    Guid CategoryId,
    string? CourseImages,
    string? CourseTags,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    string? CategoryName,
    string? CategorySlug
);

public sealed record ModuleRow(
    Guid Id,
    string Title,
    int Index,
    Guid CourseId
);

public sealed record LessonRow(
    Guid Id,
    Guid ModuleId,
    string Title,
    string? Description,
    int Index,
    TimeSpan Duration,
    string? ThumbnailImageUrl,
    string? VideoUrl,
    string Access
);

public sealed record CategoryRow(
    Guid Id,
    string Name,
    string Slug
);

public sealed record ImageUrlData(string Path);

public sealed record TagData(string Value);
