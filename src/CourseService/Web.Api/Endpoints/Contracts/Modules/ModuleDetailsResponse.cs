using Courses.Api.Endpoints.Contracts.Lessons;
using Courses.Api.Endpoints.Contracts.Shared;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Endpoints.Contracts.Modules;

internal sealed record ModuleDetailsResponse(
    Guid ModuleId,
    string Title,
    int Index,
    int LessonCount,
    TimeSpan TotalDuration,
    IReadOnlyList<LessonSummaryResponse> Lessons,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
