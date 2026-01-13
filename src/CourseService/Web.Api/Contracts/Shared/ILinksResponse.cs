using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Contracts.Shared;

internal interface ILinksResponse
{
    IReadOnlyCollection<LinkDto> Links { get; init; }
}
