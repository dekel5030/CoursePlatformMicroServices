using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Endpoints.Contracts.Shared;

internal interface ILinksResponse
{
    IReadOnlyCollection<LinkDto> Links { get; init; }
}
