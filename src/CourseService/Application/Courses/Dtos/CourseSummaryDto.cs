using System.Text.Json.Serialization;
using Courses.Application.Categories.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseSummaryDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string ShortDescription { get; init; }
    public required string Slug { get; init; }

    public required InstructorDto Instructor { get; init; }
    public required CategoryDto Category { get; init; }

    public required Money Price { get; init; }
    public required Money? OriginalPrice { get; init; }
    public required List<string> Badges { get; init; }

    public required double AverageRating { get; init; }
    public required int ReviewsCount { get; init; }

    public required string? ThumbnailUrl { get; init; }
    public required int LessonsCount { get; init; }
    public required TimeSpan Duration { get; init; }
    public required DifficultyLevel Difficulty { get; init; }

    public required int EnrollmentCount { get; init; }
    public required int CourseViews { get; init; }
    public required DateTimeOffset UpdatedAtUtc { get; init; }

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    public required CourseStatus Status { get; init; }

    public required IReadOnlyList<LinkDto> Links { get; init; }
}
