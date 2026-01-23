using Courses.Api.Endpoints.Contracts.Shared;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Endpoints.Contracts.Lessons;

internal sealed record LessonDetailsResponse(
    Guid CourseId,
    Guid LessonId,
    string Title,
    string Description,
    int Index,
    TimeSpan? Duration,
    string? ThumbnailUrl,
    string LessonAccess,
    string LessonStatus,
    string? VideoUrl,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
