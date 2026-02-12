using Courses.Application.Categories.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Users;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Features.Dtos;

public record CourseSummaryDto
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

    public required IReadOnlyList<LinkDto> Links { get; init; }
}
