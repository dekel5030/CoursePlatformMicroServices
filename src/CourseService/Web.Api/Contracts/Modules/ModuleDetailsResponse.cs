using Courses.Api.Contracts.Lessons;
using Courses.Api.Contracts.Shared;
using Courses.Application.Abstractions.Hateoas;

namespace Courses.Api.Contracts.Modules;

internal sealed record ModuleDetailsResponse(
    Guid ModuleId,
    string Title,
    int Index,
    int LessonCount,
    TimeSpan TotalDuration,
    IReadOnlyList<LessonSummaryResponse> Lessons,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
