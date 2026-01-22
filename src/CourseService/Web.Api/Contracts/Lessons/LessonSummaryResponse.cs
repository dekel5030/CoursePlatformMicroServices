using Courses.Api.Contracts.Shared;
using Courses.Application.Abstractions.Hateoas;

namespace Courses.Api.Contracts.Lessons;

internal sealed record LessonSummaryResponse(
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan? Duration,
    string? ThumbnailUrl,
    string LessonAccess,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
