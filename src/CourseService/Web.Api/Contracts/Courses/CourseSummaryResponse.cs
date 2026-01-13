using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Contracts.Courses;

internal sealed record CourseSummaryResponse(
    Guid Id,
    string Title,
    string? InstructorName,
    decimal Price,
    string Currency,
    string? ThumbnailUrl,
    int LessonsCount,
    int EnrollmentCount,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
