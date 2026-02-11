using Courses.Application.Categories.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record ManagedCourseSummaryDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string ShortDescription { get; init; }
    public required string Slug { get; init; }

    public required InstructorDto Instructor { get; init; }
    public required CategoryDto Category { get; init; }

    public required Money Price { get; init; }
    public required DifficultyLevel Difficulty { get; init; }

    public required string? ThumbnailUrl { get; init; }
    public required DateTimeOffset UpdatedAtUtc { get; init; }

    public required CourseStatus Status { get; init; }

    public required ManagedCourseStatsDto Stats { get; init; }

    public required IReadOnlyList<LinkDto> Links { get; init; }
}

public sealed record ManagedCourseStatsDto(
    int LessonsCount,
    TimeSpan Duration
);
