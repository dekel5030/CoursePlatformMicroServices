using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Api.Contracts.Lessons;

internal sealed record LessonSummaryResponse(
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan? Duration,
    string? ThumbnailUrl,
    string LessonStatus,
    string LessonAccess,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
