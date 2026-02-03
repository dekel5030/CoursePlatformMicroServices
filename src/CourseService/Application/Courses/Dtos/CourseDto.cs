using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.Dtos;

public record CourseDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required CourseStatus Status { get; init; }
    public required Money Price { get; init; }
    public required DateTimeOffset UpdatedAtUtc { get; init; }
    public required IReadOnlyList<string> ImageUrls { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }

    public required Guid InstructorId { get; init; }
    public required Guid CategoryId { get; init; }

    public required List<LinkDto> Links { get; init; }
}
