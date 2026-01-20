using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;

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
