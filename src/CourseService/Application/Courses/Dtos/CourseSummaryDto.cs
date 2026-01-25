using System.Text.Json.Serialization;
using Courses.Application.Categories.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseSummaryDto(
    Guid Id,
    string Title,
    string ShortDescription,
    string Slug,

    InstructorDto Instructor,
    CategoryDto Category,

    Money Price,
    Money? OriginalPrice,
    List<string> Badges,

    double AverageRating,
    int ReviewsCount,

    string? ThumbnailUrl,
    int LessonsCount,
    TimeSpan Duration,
    DifficultyLevel Difficulty,

    int EnrollmentCount,
    int CourseViews,
    DateTimeOffset UpdatedAtUtc,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    CourseStatus Status);
