using Courses.Api.Contracts.Shared;
using Courses.Application.Abstractions.Hateoas;

namespace Courses.Api.Contracts.Courses;

internal sealed record CourseSummaryResponse(
    Guid Id,
    string Title,
    string InstructorName,
    Guid InsturctorId,
    string? InstuctorAvatarUrl,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
