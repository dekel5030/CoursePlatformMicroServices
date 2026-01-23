using Courses.Api.Endpoints.Contracts.Shared;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Endpoints.Contracts.Courses;

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
