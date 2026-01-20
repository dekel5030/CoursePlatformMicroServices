using Courses.Api.Contracts.Lessons;
using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Contracts.Courses;

internal sealed record CourseDetailsResponse(
    Guid Id,
    string Title,
    string Description,
    string InstructorName,
    Guid InstructorId,
    string? InstructorAvatarUrl,
    string CourseStatus,
    decimal Price,
    string Currency,
    int EnrollmentCount,
    int LessonCount,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<LessonSummaryResponse> Lessons,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
