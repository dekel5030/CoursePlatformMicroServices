using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Users.Dtos;

namespace Courses.Application.Features.CourseRatings.GetCourseRatings;

public sealed record CourseRatingDto
{
    public required Guid Id { get; init; }
    public required Guid CourseId { get; init; }
    public required UserDto User { get; init; }
    public required int Rating { get; init; }
    public required string Comment { get; init; } = string.Empty;
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
    public required List<LinkDto> Links { get; init; }
}
