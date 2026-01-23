using Courses.Api.Endpoints.Contracts.Shared;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Endpoints.Contracts.Lessons;

internal sealed record LessonSummaryResponse(
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan? Duration,
    string? ThumbnailUrl,
    string LessonAccess,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
